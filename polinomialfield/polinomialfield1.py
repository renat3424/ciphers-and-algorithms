import numpy as np
import random
import math

def printPol(code):
    if code[0]==0 and code.size==1:
        return 0
    st=""
    for i in range(code.size):
        if code[i]==0:
            continue
        else:
            if i==code.size-1:
                st+=str(code[i])
            else:
                if code[i]!=1:
                    st+=str(code[i])+"α^"+str(code.size-1-i)+"+"
                else:
                    st += "α^" + str(code.size - 1 - i) + "+"

    if st[-1]=="+":
        st=st[:-1]
    return st


def get_addition_table(p):
    table=np.zeros((p, p))
    for i in range(p):
        for j in range(p):
            table[i, j]=(i+j)%p
    return table


def get_multiplication_table(p):
    table=np.zeros((p, p))
    for i in range(p):
        for j in range(p):
            table[i, j]=(i*j)%p
    return table

def get_extended_field(p, primitive_polinom, m):
    ext_field=[np.array([1])]
    for i in range(1, p**m):
        mult_pol=np.polymul(ext_field[-1], np.array([1, 0]))
        ext_field.append((np.polydiv(mult_pol, primitive_polinom)[1] % p).astype(int))
    return ext_field


def get_log_table(p, primitive_polinom, ext_field, m):
    log_table=["infinity"]
    for i in range(1, p**m-1):
        pol=np.zeros(i+1)
        pol[0]=1
        pol[-1]=1
        rem=(np.polydiv(pol, primitive_polinom)[1] % p).astype(int)
        for j in range(len(ext_field)):
            if np.poly1d(rem)==np.poly1d(ext_field[j]):
                log_table.append(j)
    return np.array(log_table)


def multiply(i, j, ext_field, p, m):
    if i=="infinity":
        return "infinity"
    if j=="infinity":
        return "infinity"

    return (i + j) % (p ** m - 1)


def sum(i, j, ext_field, p, m, log_table):

    if i == "infinity" and j != "infinity":
        return j
    elif j == "infinity" and i != "infinity":
        return i
    if i == "infinity" and j == "infinity":
        return "infinity"
    if log_table[abs(i - j)% (p ** m - 1)] == "infinity":
        return "infinity"

    return (j + int(log_table[abs(i - j)% (p ** m - 1)])) % (p ** m - 1)


def print_table(log_table):
    print("Logarithmic table:")
    for i in range(len(log_table)):
        print(str(i)+": "+str(log_table[i]))

#Задание p=5, m=3

def generator_poly(d, ext_field, p, m, log_table):
    gx = [0]
    for index in range(1, d):
        gx = mul_poly(gx, [0, index], ext_field, p, m, log_table)
    return gx

def mul_poly(polynom1, polynom2, ext_field, p, m, log_table):
    result = ["infinity"]
    result *= (len(polynom1) + len(polynom2) - 1)
    for i, ci in enumerate(polynom1):
        for j, cj in enumerate(polynom2):
            first = result[i + j]
            second = multiply(ci, cj, ext_field, p, m)
            result[i + j] = sum(first, second, ext_field, p, m, log_table)
    for i in range(len(result) - 1):
        if result[0] != "infinity":
            break
        else:
            result = result[1:]
    return result

def add_poly(polynom1, polynom2, log_table, ext_field, p, m):
    if (len(polynom2) - 1) > (len(polynom1) - 1):
        result = polynom2.copy()
    if (len(polynom2) - 1) <= (len(polynom1) - 1):
        result = polynom1.copy()
    stop = min(len(polynom1) - 1, len(polynom2) - 1) + 2
    for i in range(1, stop):
        result[-i] = sum(polynom1[-i], polynom2[-i], ext_field, p, m,
                            log_table)
    for i in range(len(result) - 1):
        if result[0] != "infinity":
            break
        else:
            result = result[1:]
    return result

def mod_poly(polynom1, polynom2, ext_field, p, m, log_table):
    if (len(polynom2) - 1) > (len(polynom1) - 1):
        return polynom1
    first_copy, second_copy = polynom1.copy(), polynom2.copy()
    result = []
    while (len(first_copy) - 1) > (len(second_copy) - 1) - 1:
        result.append(np.mod(first_copy[0] - second_copy[0], p ** m - 1))
        temp = mul_poly(second_copy, [np.mod(first_copy[0] - second_copy[0], p ** m - 1)] + ["infinity"] * (
                (len(first_copy) - 1) - (len(second_copy) - 1)), ext_field, p, m, log_table)
        first_copy = add_poly(first_copy, temp, log_table, ext_field, p, m)
    return first_copy


def check_poly(d, n, ext_field, p, m, log_table):
    hx = [0]
    for index in range(d, n + 1):
        hx = mul_poly(hx, [0, index], ext_field, p, m, log_table)
    return hx


def generator_mtrx(n, d, k, gxc, ext_field, p, m, log_table):
    G_mtrx = []
    for i, j in enumerate(range(n - 1, d - 2, -1)):
        v = ["infinity"] * k
        v[i] = 0
        v += mod_poly(([0] + ["infinity"] * j), gxc, ext_field, p, m, log_table)
        G_mtrx.append(v)
    return list(map(list, zip(*G_mtrx)))


def check_mtrx(d, p, m):
    H_mtrx = []
    for i in range(1, d):
        temporary = [i * j for j in range(p**m - 1)]
        H_mtrx.append(temporary)
    return H_mtrx


p=2
m=5

print(get_addition_table(p))
print(get_multiplication_table(p))
primitive_polinom=np.array([1, 0,0, 1, 0, 1])
primitive_element=(-1*primitive_polinom[1:])%p
print(printPol(primitive_polinom))
print(printPol(primitive_element))
ext_field=get_extended_field(p, primitive_polinom, m)
for i in range(p**m):
    print("α^"+str(i)+"="+printPol(ext_field[i]))

log_table=get_log_table(p, primitive_polinom, ext_field, m)
print_table(log_table)


ns, ks, ds = 255, 247, 9
ts = (ds - 1) // 2

g_poly = generator_poly(ds, ext_field, p, m, log_table)
print(f'Порождающий многочлен g(x) степени n-k (как вектор из степеней примитивного элемента):\n{g_poly}')
c_poly = check_poly(ds, ns, ext_field, p, m, log_table)
print(f'Проверочный многочлен:\n{c_poly}')
g_mtrx = generator_mtrx(ns, ds, ks, g_poly, ext_field, p, m, log_table)
# print(f'Порождающая матрица:')
# for row in G_mtrx:
#     print(row)
c_mtrx = check_mtrx(ds, p, m)
# print(f'Проверочная матрица:')
# for row in H_mtrx:
#     print(row)


