import numpy as np

np.random.seed(2361)


class rs_code:
    def __init__(self, n, k, d, short_diff, m, p, q, primitive_poly):
        self.n, self.k, self.d = n, k, d
        self.short_diff = short_diff
        self.r = d - 1
        self.m = m
        self.p = p
        self.q = q
        self.Fx = np.poly1d(primitive_poly)
        self.Fx_gen = np.poly1d(np.mod((self.Fx.coef[1:] * (-1)), p))
        self.elements = self.get_elements(self.Fx, self.Fx_gen)
        self.logarithms = self.get_logarithm_table()
        ds = d + 1
        ts = (ds - 1) // 2
        ks = k + short_diff
        ns = ds + ks - 1
        self.gxs = self.generator_poly(ds, self.q, self.logarithms)
        self.hxs = self.parity_poly(ds, ns, self.q, self.logarithms)
        self.Gs = self.generator_matrix(ns, ds, ks, self.gxs, q, self.logarithms)
        self.Hs = self.get_H(ds, q)
        self.Mx = np.random.randint(0, q - 1, k).tolist()
        self.Cx = self.encode(self.Mx, short_diff, q, self.logarithms, ns, self.Gs, ks)
        e = ['-∞'] * (n)
        e[14] = 8
        e[7] = 13
        Ex = e[::-1]
        Vx = self.add_polynomials_gf(self.Cx, Ex, self.logarithms, q)
        Sx = self.get_syndrome(Vx, ks, short_diff, ts, self.Hs)

        LLx = self.get_locator(Sx, ds, q, self.logarithms)
        pos = np.array(
            self.get_error_position(ns, LLx, q, self.logarithms, short_diff))
        SSx = Sx[::-1]
        print(f'Параметры укороченного кода: ({n},{k},{d})\n'
              f'Параметры основного кода: ({n + short_diff},{k + short_diff},{d + 1})')
        print("Кодируемое сообщение")
        print(self.to_string(self.Mx))
        print("Порождающий многочлен")
        print(self.to_string(self.gxs))
        print("Порождающая матрица")
        for row in self.Gs:
            print(row)
        print("Проверочная матрица")
        for row in self.Hs:
            print(row)
        print("Закодированное слово")
        print(self.to_string(self.Cx))
        print(f'Полином ошибки: {self.to_string(Ex)}')
        print(ns, ks, ds)
        print(f'Полином с ошибкой: {self.to_string(Vx)}')
        print('Синдромы:')
        for i, s in enumerate(Sx):
            print(f'S_{i + 1} = {"α^" + str(s)}')
        print(f'Полином локатора ошибок: {self.to_string(LLx)}')
        print(f'Полином ошибки: {self.to_string(Ex)}')
        print(f'Позиции ошибок: {pos}')
        print(f'Величины ошибок: {self.get_error_values(pos, SSx, LLx, p, q, self.logarithms, ds, short_diff)}')

    def get_logarithm_table(self):
        table_logarithms = {0: '-∞'}
        for s in range(1, self.q - 1):
            add_res = np.mod(np.polyadd(self.elements[s], [1]), self.p)
            add_res = np.int16(add_res).tolist()

            for key, value in self.elements.items():
                if add_res == value:
                    table_logarithms[s] = key
                    break
        return table_logarithms

    def get_elements(self, fx, fx_generator):
        elems = {}

        for i in range(self.m):
            vec = np.zeros(self.m, dtype=np.int32)
            vec = vec.tolist()
            vec[self.m - i - 1] = 1
            elems[i] = vec

        elems[self.m] = (self.m - fx_generator.coef.shape[0]) * [0]
        elems[self.m] += fx_generator.coef.tolist()

        for i in range(self.m + 1, self.q - 1):
            _, remainder = np.polydiv(np.polymul(elems[i - 1], elems[1]), fx)
            elems[i] = [0] * (self.m - np.mod(remainder,
                                              self.p).shape[0]) + np.int16(np.mod(remainder, self.p)).tolist()
        return elems

    def add_powers_gf(self, power1, power2, q, table_logarithms):
        if power1 == '-∞' and power2 != '-∞':
            return power2
        elif power2 == '-∞' and power1 != '-∞':
            return power1

        if power1 == '-∞' and power2 == '-∞':
            return '-∞'

        if power1 > power2:
            power1, power2 = power2, power1

        if table_logarithms[(power2 - power1) % (q - 1)] == '-∞':
            return '-∞'
        return np.mod(power1 + table_logarithms[(power2 - power1) % (q - 1)], q - 1)

    def mult_powers_gf(self, power1, power2, q):

        if power1 == '-∞':
            return '-∞'

        if power2 == '-∞':
            return '-∞'

        return (power1 + power2) % (q - 1)

    def add_polynomials_gf(self, polynom1, polynom2, table_logarithms, q):

        if (len(polynom2) - 1) > (len(polynom1) - 1):
            result = polynom2.copy()

        if (len(polynom2) - 1) <= (len(polynom1) - 1):
            result = polynom1.copy()

        stop = min(len(polynom1) - 1, len(polynom2) - 1) + 2

        for i in range(1, stop):
            result[-i] = self.add_powers_gf(polynom1[-i], polynom2[-i], q, table_logarithms)

        for i in range(len(result) - 1):
            if result[0] != '-∞':
                break
            else:
                result = result[1:]
        return result

    def to_string(self, polynomial):

        result = ''.join(
            ['α^' + str(c) + '*x^' + str(len(polynomial) - 1 - k) + '+' for k, c in enumerate(polynomial[:-1]) if
             c != '-∞'])
        return result + 'α^' + str(polynomial[-1]) if polynomial[-1] != '-∞' else result[:-1]

    def mult_polynomials_gf(self, polynom1, polynom2, q, table_logarithms):

        result = ['-∞']
        result *= (len(polynom1) + len(polynom2) - 1)

        for i, ci in enumerate(polynom1):
            for j, cj in enumerate(polynom2):
                first = result[i + j]
                second = self.mult_powers_gf(ci, cj, q)
                result[i + j] = self.add_powers_gf(first, second, q, table_logarithms)

        for i in range(len(result) - 1):
            if result[0] != '-∞':
                break
            else:
                result = result[1:]

        return result

    def divide_polynomials_gf(self, polynom1, polynom2, q, table_logarithms):

        first_copy, second_copy = polynom1.copy(), polynom2.copy()

        if (len(second_copy) - 1) > (len(first_copy) - 1):
            return first_copy

        if (len(second_copy) - 1) == 0:
            return ['-∞' if p_i == '-∞' else np.mod(p_i - second_copy[-1], q - 1) for p_i in first_copy] if second_copy[
                                                                                                                -1] != '-∞' else first_copy

        result = []

        while (len(first_copy) - 1) > (len(second_copy) - 1) - 1:
            result.append((first_copy[0] - second_copy[0]) % (q - 1))
            first_copy = self.add_polynomials_gf(first_copy, self.mult_polynomials_gf(second_copy, [
                (first_copy[0] - second_copy[0]) % (q - 1)] + ['-∞'] * (
                                                                                              (len(first_copy) - 1) - (
                                                                                              len(second_copy) - 1)), q,
                                                                                      table_logarithms),
                                                 table_logarithms, q)
        return result

    def polynomial_mod(self, polynom1, polynom2, q, table_logarithms):

        if (len(polynom2) - 1) > (len(polynom1) - 1):
            return polynom1

        first_copy, second_copy = polynom1.copy(), polynom2.copy()
        result = []

        while (len(first_copy) - 1) > (len(second_copy) - 1) - 1:
            result.append(np.mod(first_copy[0] - second_copy[0], q - 1))
            temp = self.mult_polynomials_gf(second_copy, [np.mod(first_copy[0] - second_copy[0], q - 1)] + ['-∞'] * (
                    (len(first_copy) - 1) - (len(second_copy) - 1)), q, table_logarithms)
            first_copy = self.add_polynomials_gf(first_copy, temp, table_logarithms, q)

        return first_copy

    def get_value_poly(self, x, coefficients, q, table_logarithms):

        v = '-∞'
        for index in range(1, len(coefficients) + 1):
            v = self.add_powers_gf(v, self.mult_powers_gf(coefficients[-index], x * (index - 1), q), q,
                                   table_logarithms)
        return v

    def poly_derivative(self, c, p):
        deriv_coef = ['-∞' if np.mod(len(c) - 1 - index, p) == 0 else c[index] for index in range(len(c) - 1)]

        for index in range(len(deriv_coef) - 1):
            if deriv_coef[0] != '-∞':
                break
            else:
                deriv_coef = deriv_coef[1:]

        return deriv_coef

    def generator_poly(self, d, q, table_logarithms):

        gx = [0]
        for index in range(1, d):
            gx = self.mult_polynomials_gf(gx, [0, index], q, table_logarithms)
        return gx

    def parity_poly(self, d, n, q, table_logarithms):
        hxc = [0]
        for index in range(d, n + 1):
            hxc = self.mult_polynomials_gf(hxc, [0, index], q, table_logarithms)
        return hxc

    def generator_matrix(self, n, d, k, gxc, q, table_logarithms):

        G = []

        for i, j in enumerate(range(n - 1, d - 2, -1)):
            v = ['-∞'] * k
            v[i] = 0
            v += self.polynomial_mod(([0] + ['-∞'] * j), gxc, q, table_logarithms)
            G.append(v)

        return list(map(list, zip(*G)))

    def get_H(self, d, q):

        H = []

        for i in range(1, d):
            temporary = [i * j for j in range(q - 1)]
            H.append(temporary)

        return H

    def encode(self, mx, shortened_code, q, table_logarithms, n, G, k):

        mx = self.mult_polynomials_gf((['-∞'] * shortened_code + [0]), mx, q, table_logarithms)
        result = []

        for i in range(n):
            value = '-∞'

            for j in range(min(len(G[i]), len(mx) - 1) + 1):
                value = self.add_powers_gf(value, self.mult_powers_gf(G[i][j], mx[j], q), q, table_logarithms)
            result.append(value)

        res_coef = result[:k - shortened_code] + result[k:]
        return res_coef

    def get_syndrome(self, mxc, k, shortened_code, t, H):
        mx = mxc[:k - shortened_code] + ['-∞'] * shortened_code + mxc[k - shortened_code:]
        result = []

        for i in range(t * 2):
            value = '-∞'
            length = min(len(H[i][::-1]), len(mx) - 1)

            for j in range(length + 1):
                value = self.add_powers_gf(value, self.mult_powers_gf(H[i][::-1][j], mx[j], self.q), self.q,
                                           self.logarithms)

            result.append(value)
        return result

    def get_locator(self, p_synd, d, q, table_logarithms):
        i, L = 0, 0
        p_lx, t_p = [0], [0]
        while i < d - 1:
            i += 1
            delta = '-∞'
            for j in range(min(L + 1, len(p_lx))):
                delta = self.add_powers_gf(delta, self.mult_powers_gf(p_lx[-j - 1], p_synd[i - j - 1], q),
                                           q, table_logarithms)
            if delta == '-∞':
                t_p = self.mult_polynomials_gf(t_p, [0, '-∞'], q, table_logarithms)
                continue
            Tx = self.add_polynomials_gf(p_lx, self.mult_polynomials_gf(
                self.mult_polynomials_gf(t_p, [0, '-∞'], q, table_logarithms),
                [delta], q, table_logarithms), table_logarithms, q)
            if 2 * L + 1 > i:
                p_lx = Tx.copy()
                t_p = self.mult_polynomials_gf([0, '-∞'], t_p, q, table_logarithms)
                continue
            t_p = self.divide_polynomials_gf(p_lx, [delta], q, table_logarithms)
            p_lx = Tx.copy()
            L = i - L
        return p_lx

    def get_error_position(self, n, lx, q, table_logarithms, shortened_code):

        return [np.mod(-i - shortened_code, q - 1) for i in range(n) if
                str(self.get_value_poly(i, lx, q, table_logarithms)) == '-∞']

    def get_error_values(self, position, synd, lx, p, q, table_logarithms, d, shortened_code):
        val = []
        sigma = self.polynomial_mod(self.mult_polynomials_gf(synd, lx, q, table_logarithms), ([0] + ['-∞'] * (d - 1)),
                                    q,
                                    table_logarithms)
        for i in position:
            val.append(
                np.mod(
                    self.get_value_poly(q - 1 - i - shortened_code, sigma, q, table_logarithms) - self.get_value_poly(
                        q - 1 - i - shortened_code, self.poly_derivative(lx, p), q, table_logarithms), q - 1))
        return val


if __name__ == '__main__':
    n, k, d = 28,18,10
    short_diff = 3
    m, p, q = 5, 2, 2 ** 5
    primitive_poly = [1, 0, 0, 1, 0, 1]
    rs = rs_code(n, k, d, short_diff, m, p, q, primitive_poly)

