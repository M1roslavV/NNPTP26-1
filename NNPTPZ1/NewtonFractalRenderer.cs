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
                        float iterations;
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
                Re = options.XMin + j * options.XStep,
                Imaginari = (float)(options.YMin + i * options.YStep)
            };

            if (point.Re == 0)
                point.Re = 0.0001;
            if (point.Imaginari == 0)
                point.Imaginari = 0.0001f;

            return point;
        }

        private Cplx FindRoot(Cplx point, out float iterations)
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
            return Math.Pow(value.Re, 2) + Math.Pow(value.Imaginari, 2);
        }

        private static Color GetPixelColor(int rootIndex, float iterations)
        {
            Color color = RootColors[rootIndex % RootColors.Length];
            int darkening = (int)iterations * DarkeningPerIteration;
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
