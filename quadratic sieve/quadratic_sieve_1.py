import sympy as sp
from sympy import sieve
from sympy import Matrix
import numpy as np
from sympy.ntheory import legendre_symbol

# Получение числа B для проверки гладкости
def getB(N):
    return int(np.floor(pow(pow(np.e, np.sqrt(np.log(N) * np.log(np.log(N)))), 1 / np.sqrt(2))))


# Получение массива простых чисел до B
def getPrimeNums(B):
    return np.array(list(sieve.primerange(B)))



# Квадратичное решето
def quadratic_sieve(N, M=None, B=None):
    # Генерируем B
    if (B == None):
    	#при помощи функции иногда данного числа может не хватить, можно ставить множители или ввести B самому
        B = 40*getB(N)

    # Получаем начальный  x
    x_init = int(np.floor(np.sqrt(N))) + 1

    primes = getPrimeNums(B)
    print('primes:', primes)
    primes1=[]
    # находим простые числа такие, что символ лежандра n/p==1
    for p in primes[1:]:

        if(legendre_symbol(N, p)==1):
            primes1.append(p)
    primes=np.array(primes1)
    print('M: ', M)
    # Получаем последовательность чисел от sqrt(N)+1 до M который вводим сами 
    x_s = np.arange(x_init, M).astype(np.longlong)
    #Строим данную последоваетльность из предыдущей то есть x^2-N
    y_s_init = [(x ** 2 - N) for x in x_s]

    # Просеивание

    print('primes:', primes)
    A = []
    y_t = y_s_init.copy()
    y_s = []
    x_s1 = []

    # Просеиваем числа y и формируем A
    # Берем число из полученной последовательности, берем простое число из полученных простых чисел делим число из последовательности на простое число, полученное количество делений записываем в массив для всех простых чисел, этот массив добавляем в А
    for i in range(len(y_t)):
        p_amount = [0 for p in primes]
        for j in range(len(primes)):
            while (y_t[i] % primes[j] == 0):
                y_t[i] /= primes[j]
                p_amount[j] += 1
        if (y_t[i] == 1):
            A.append(p_amount)
            y_s.append(y_s_init[i])
            x_s1.append(x_s[i])

    # Транспонируем, удаляем нулевые строки и оставляем
    # только четность встречаемости простых чисел (получаем матрицу нулей и единиц где количество столбцов количество простых чисел, количество строк количество чисел последовательности)
    A = np.array(A).T
    A_new = []
    for i in range(len(A)):
        if np.sum(A[i]) != 0:
            A_new.append(A[i] % 2)
    A_new = np.array(A_new)
    print(A_new)


    #находим линейно независимые строки в матрице
    M=A_new[Matrix(A_new).T.rref()[1], :]%2

    print('Matrix: ', M)
    #находим количество свободных переменных в матрице отнимая количество переменных(столбцов), от количества строк
    numVals = M.shape[1] - M.shape[0]
    print(numVals)
    #если количество свободных переменных нулевое матрица квадратная, а значит просто решаем систему линейных уравнений для квадратной матрицы
    if numVals==0:
        x = np.linalg.solve(M, np.zeros(M.shape[0])).astype(int)%2
        #если получаем нулевое решение возвращаем ноль
        if (x==0).all():
            return 0
        #находим числа из двух последовательностей для полученного решения, то есть, если в строке стоит 1 выбираем эти числа из последовательностей
        y = y_s[(x == 1)].astype(np.float64)
        X = x_s1[(x == 1)].astype(np.float64)
        #находим произведение этих чисел из первой последовательности и корень произведения чисел из второй и отнимаем полученные произведения 
        num = (X.prod() - np.sqrt(y.prod())).astype(np.longlong)
        # находим НОД факторизуемого числа и полученного числа из предыдущей строки, если он не равен 1 или факторизуемому числу возвращаем его
        gcd = np.gcd(num, N)
        if (gcd != 1 and gcd != N):
            return gcd
        return 0

    #находим линейно независимые столбцы из матрицы нулей и единиц
    j_s = Matrix(M).rref()[1]
    #создаем из них столбцы
    M1=M[:, j_s]


    j_s = np.array(j_s)
    print('j_s: ', j_s)
    #оставшиеся столбцы вставляем в другую матрицу
    M2 = np.delete(M, j_s, 1)
    
    print('M1: ', M1)
    print('M2: ', M2)
    y_s = np.array(y_s)
    x_s1 = np.array(x_s1)
    #перебираем все возможные варианты свободных переменных от 0 до 2^(количество свободных переменных) (2 переменные варианты 00 01 10 11)
    for i in range(pow(2, numVals)):
    	#переводим вариант в двоичный код создаем массив из него
        array = np.array([int(x) for x in bin(i)[2:]])
        #pad до количества свободных переменных так чтобы массив всегда был одного размера
        array = np.pad(array, (numVals - array.shape[0], 0)).astype(int)
        #умножаем коэффициенты при свободных переменных, которые находятся в матрице 2 на свободные переменные
        ans = np.array((M2 @ array) % 2, int)
        #решаем систему
        x = np.linalg.solve(M1, ans).astype(int)
        #получаем одно из решений
        solution = np.zeros(M.shape[1])
        #создаем массив решения добавляя в него свободные переменные и полученные решения
        solution[j_s] = x
        solution[np.delete(range(M.shape[1]), j_s, 0)] = array
        solution=solution%2
        #находим для данного возможного решения Наибольший общий делитель как было написано раннее
        y = y_s[(solution == 1)].astype(np.float64)
        X = x_s1[(solution == 1)].astype(np.float64)

        num = (X.prod() - np.sqrt(y.prod())).astype(np.longlong)
        gcd = np.gcd(num, N)

        if (gcd != 1 and gcd != N):
            return gcd



    #если нет решений возвращаем ноль
    return 0


if __name__ == '__main__':
    print(quadratic_sieve(239249,3000))