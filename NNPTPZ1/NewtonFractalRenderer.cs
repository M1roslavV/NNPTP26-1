using System;
using System.Collections.Generic;
using System.Drawing;
using NNPTPZ1.Mathematics;

namespace NNPTPZ1
{
    internal sealed class NewtonFractalRenderer
    {
        private const int RequiredIterations = 30;
        private const double LargeStepSquared = 0.5;
        private const double RootToleranceSquared = 0.01;
        private const int DarkeningPerIteration = 2;

        private static readonly Color[] RootColors =
        {
            Color.Red, Color.Blue, Color.Green, Color.Yellow, Color.Orange,
            Color.Fuchsia, Color.Gold, Color.Cyan, Color.Magenta
        };

        private readonly Poly polynomial;
        private readonly Poly derivative;

        public NewtonFractalRenderer(Poly polynomial, Poly derivative)
        {
            this.polynomial = polynomial;
            this.derivative = derivative;
        }

        public Bitmap Render(RenderOptions options)
        {
            var bitmap = new Bitmap(options.Width, options.Height);
            var roots = new List<Cplx>();
            try
            {
                // Preserve the original traversal and coordinate mapping.
                for (int i = 0; i < options.Width; i++)
                {
                    for (int j = 0; j < options.Height; j++)
                    {
                        Cplx point = CreateStartingPoint(options, i, j);
                        int iterations;
                        Cplx root = FindRoot(point, out iterations);
                        int rootIndex = FindRootIndex(roots, root);
                        bitmap.SetPixel(j, i, GetPixelColor(rootIndex, iterations));
                    }
                }
                return bitmap;
            }
            catch
            {
                bitmap.Dispose();
                throw;
            }
        }

        private static Cplx CreateStartingPoint(RenderOptions options, int i, int j)
        {
            var point = new Cplx
            {
                Real = options.XMin + j * options.XStep,
                Imaginary = (float)(options.YMin + i * options.YStep)
            };

            if (point.Real == 0)
                point.Real = 0.0001;
            if (point.Imaginary == 0)
                point.Imaginary = 0.0001f;

            return point;
        }

        private Cplx FindRoot(Cplx point, out int iterations)
        {
            iterations = 0;
            for (int step = 0; step < RequiredIterations; step++)
            {
                Cplx difference = polynomial.Eval(point).Divide(derivative.Eval(point));
                point = point.Subtract(difference);

                // Large steps do not count toward the original iteration target.
                if (SquaredMagnitude(difference) >= LargeStepSquared)
                    step--;
                iterations++;
            }
            return point;
        }

        private static int FindRootIndex(List<Cplx> roots, Cplx root)
        {
            int index = -1;
            for (int i = 0; i < roots.Count; i++)
            {
                if (SquaredMagnitude(root.Subtract(roots[i])) <= RootToleranceSquared)
                    index = i;
            }

            if (index >= 0)
                return index;

            roots.Add(root);
            // Keep the original color assignment for the first occurrence of a root.
            return roots.Count;
        }

        private static double SquaredMagnitude(Cplx value)
        {
            return Math.Pow(value.Real, 2) + Math.Pow(value.Imaginary, 2);
        }

        private static Color GetPixelColor(int rootIndex, int iterations)
        {
            Color color = RootColors[rootIndex % RootColors.Length];
            int darkening = iterations * DarkeningPerIteration;
            return Color.FromArgb(
                DarkenChannel(color.R, darkening),
                DarkenChannel(color.G, darkening),
                DarkenChannel(color.B, darkening));
        }

        private static int DarkenChannel(int channel, int darkening)
        {
            return Math.Min(Math.Max(0, channel - darkening), 255);
        }
    }
}
