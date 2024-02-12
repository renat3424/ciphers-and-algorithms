import numpy as np
import random
import math

def ascii(text):
    value = ord(text)

    bnr = bin(value)[2:]
    return bnr.zfill(8)

def asciiToArray(text):
    array=[]
    for i in text:
        if (i=="1"):
            array.append(1)
        else:
            array.append(0)
    return np.array(array)


def shift(code, k):
    res = np.copy(code)
    for i in range(k):
        res = np.insert(res, code.size, 0)
    return res

def printPol(code):
    if code[0]==0 and code.size==1:
        return 0
    st=""
    for i in range(code.size):
        if code[i]==0:
            continue
        else:
            if i==code.size-1:
                st+="1"
            else:
                    st+="x^"+str(code.size-1-i)+"+"

    if st[-1]=="+":
        st=st[:-1]
    return st

def sumPol(a, b):
    return np.abs(np.polyadd(a, b)%2).astype(int)

def divPoly(a, b):
    return np.abs(np.polydiv(a, b)[1]%2).astype(int)

def make_mistake(ham1):
    nums=np.random.randint(0, 13)



    ham1[nums] = abs(ham1[nums] - 1)
    return ham1




alph=["Q", "R", "S", "T", "U"]
start_pol=np.array([1, 0, 1, 0, 0, 1])
for i in range(5):
    A=asciiToArray(ascii(alph[i]))
    print("Многочлен буквы "+alph[i], end=": ")
    print(printPol(A))
    print()
    print("Порождающий многочлен ", end=": ")
    print(printPol(start_pol))
    print()
    print("Сдвинутый многочлен ", end=": ")
    Ashifted=shift(A, 5)
    print(printPol(Ashifted))
    print()
    print("Остаток от деления ", end=": ")
    mod=divPoly(Ashifted, start_pol)
    sum=sumPol(Ashifted, mod)

    print(printPol(mod))
    print()
    print("Кодовый многочлен ", end=": ")
    print(printPol(sum))
    print()
    sindrom=divPoly(sum, start_pol)
    print("Синдром без ошибки: ", end=": ")
    print(sindrom)
    print()
    print("Кодовый многочлен с ошибкой: ", end=": ")
    sum=make_mistake(sum)
    print(printPol(sum))
    print()
    sindrom=divPoly(sum, start_pol)
    print("Синдром c ошибкой: ", end=": ")
    print(printPol(sindrom))
    print()
