import numpy as np
import random
import math
def ascii(text):
    value = ord(text)

    bnr = bin(value)[2:]
    return bnr.zfill(8)

def from_ascii(asc):
    string_ints = [str(int(integ)) for integ in asc]
    str_of_ints = "".join(string_ints)
    binary_int = int(str_of_ints, 2)
    byte_number = binary_int.bit_length() + 7 // 8
    binary_array = binary_int.to_bytes(byte_number, "big")
    ascii_text = binary_array.decode()
    return ascii_text

def hamming_code(text):
    ham=np.zeros(13)
    ham[2]=int(text[0])
    ham[4]=int(text[1])
    ham[5] = int(text[2])
    ham[6] = int(text[3])
    ham[8] = int(text[4])
    ham[9] = int(text[5])
    ham[10] = int(text[6])
    ham[11] = int(text[7])
    ham[0]=(ham[2]+ham[4]+ham[6]+ham[8]+ham[10])%2
    ham[1]=(ham[2]+ham[5]+ham[6]+ham[9]+ham[10])%2
    ham[3] = (ham[4] + ham[5] + ham[6] + ham[11]) % 2
    ham[7] = (ham[8] + ham[9] + ham[10] + ham[11]) % 2
    ham[12]=ham.sum()%2
    return list(ham)

def from_hamming(ham):
    text = np.zeros(8)
    text[0] = ham[2]
    text[1] = ham[4]
    text[2] = ham[5]
    text[3] = ham[6]
    text[4] = ham[8]
    text[5] = ham[9]
    text[6] = ham[10]
    text[7] = ham[11]
    return list(text)

def check(ham1):
    ham=np.array(ham1.copy())
    ham[0] = (ham1[2] + ham1[4] + ham1[6] + ham1[8] + ham1[10]) % 2
    ham[1] = (ham1[2] + ham1[5] + ham1[6] + ham1[9] + ham1[10]) % 2
    ham[3] = (ham1[4] + ham1[5] + ham1[6] + ham1[11]) % 2
    ham[7] = (ham1[8] + ham1[9] + ham1[10] + ham1[11]) % 2
    ham[12] = sum(ham1) % 2
    S=0
    for i in range(12):
        if ham1[i]!=ham[i]:
            S+=i+1

    if(S==0 and ham[12]==0):
        print("no mistake.", end=" ")
        return from_hamming(ham1)
    elif(S!=0 and ham[12]==1):
        ham1[S-1]=abs(ham1[S-1]-1)
        print("one mistake was made.", end=" ")
        return from_hamming(ham1)
    else:
        print("two mistakes were made.", end=" ")
        return from_hamming(ham1)

def Join(asc):
    string_ints = [str(int(integ)) for integ in asc]
    str_of_ints = "".join(string_ints)
    return str_of_ints
def make_mistake(ham1, number):
    nums=random.sample(range(13), number)
    for num in nums:

        ham1[num] = abs(ham1[num] - 1)
    return ham1

def Encode():
    start_char = ord("Q")
    while (start_char <= ord("U")):
        print(chr(start_char), end=" ")
        code = ascii(chr(start_char))
        print("ASCII: "+code, end=" HAMMING: ")
        code = hamming_code(ascii(chr(start_char)))
        print(code, end=" ")
        print()
        start_char += 1
def hammingcode(number):
    start_char=ord("Q")
    while(start_char<=ord("U")):
        print(chr(start_char), end=" ")
        code=hamming_code(ascii(chr(start_char)))
        print(Join(code), end=" ")
        code=make_mistake(code, number)
        print(Join(code), end=" ")
        print("Changed value: "+from_ascii(check(code)), end=" ")
        print()
        start_char+=1

Encode()








