using System.Collections.Generic;

namespace NNPTPZ1.Mathematics
{
    public class Poly
    {
        /// <summary>
        /// Coefficients in ascending power order: index i contains the coefficient of x^i.
        /// </summary>
        public List<Cplx> Coefficients { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public Poly() => Coefficients = new List<Cplx>();

        public void Add(Cplx coefficient) =>
            Coefficients.Add(coefficient);

        /// <summary>
        /// Derives this polynomial and creates new one
        /// </summary>
        /// <returns>Derivated polynomial</returns>
        public Poly Derive()
        {
            Poly p = new Poly();
            for (int q = 1; q < Coefficients.Count; q++)
            {
                p.Coefficients.Add(Coefficients[q].Multiply(new Cplx() { Real = q }));
            }

            return p;
        }

        /// <summary>
        /// Evaluates polynomial at given point
        /// </summary>
        /// <param name="x">point of evaluation</param>
        /// <returns>y</returns>
        public Cplx Eval(double x)
        {
            var y = Eval(new Cplx() { Real = x, Imaginary = 0 });
            return y;
        }

        /// <summary>
        /// Evaluates polynomial at given point
        /// </summary>
        /// <param name="x">point of evaluation</param>
        /// <returns>y</returns>
        public Cplx Eval(Cplx x)
        {
            Cplx s = Cplx.Zero;
            for (int i = 0; i < Coefficients.Count; i++)
            {
                Cplx coef = Coefficients[i];
                Cplx bx = x;
                int power = i;

                if (i > 0)
                {
                    for (int j = 0; j < power - 1; j++)
                        bx = bx.Multiply(x);

                    coef = coef.Multiply(bx);
                }

                s = s.Add(coef);
            }

            return s;
        }

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns>String repr of polynomial</returns>
        public override string ToString()
        {
            string s = "";
            int i = 0;
            for (; i < Coefficients.Count; i++)
            {
                s += Coefficients[i];
                if (i > 0)
                {
                    int j = 0;
                    for (; j < i; j++)
                    {
                        s += "x";
                    }
                }
                if (i+1<Coefficients.Count)
                s += " + ";
            }
            return s;
        }
    }
}
