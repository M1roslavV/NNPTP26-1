using System;
using NNPTPZ1.Mathematics;

namespace NNPTPZ1
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            RenderOptions options = RenderOptions.Parse(args);
            Poly polynomial = CreatePolynomial();
            Poly derivative = polynomial.Derive();

            Console.WriteLine(polynomial);
            Console.WriteLine(derivative);

            var renderer = new NewtonFractalRenderer(polynomial, derivative);
            using (var bitmap = renderer.Render(options))
            {
                bitmap.Save(options.OutputPath);
            }
        }

        private static Poly CreatePolynomial()
        {
            var polynomial = new Poly();
            polynomial.Add(new Cplx { Re = 1 });
            polynomial.Add(Cplx.Zero);
            polynomial.Add(Cplx.Zero);
            polynomial.Add(new Cplx { Re = 1 });
            return polynomial;
        }
    }
}
