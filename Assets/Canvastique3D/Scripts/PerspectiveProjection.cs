using System;
using System.Collections.Generic;
using UnityEngine;

namespace Canvastique3D
{
    // Handles the perspective projection transformation of the canvas.
    public class PerspectiveProjection : MonoBehaviour
    {
        // Lists to store the source and destination points
        private List<Vector2> srcPts = new List<Vector2>();
        private List<Vector2> dstPts = new List<Vector2>();

        // Initializes the source points matrix
        public void InitializeMatrix()
        {
            srcPts.Clear();

            // Create a list of 4 vectors to hold the normalized coordinates of the screen's corners
            srcPts.Add(new Vector2(0f, 0f));
            srcPts.Add(new Vector2(1f, 0f));
            srcPts.Add(new Vector2(0f, 1f));
            srcPts.Add(new Vector2(1f, 1f));
        }

        // Generates and returns the transformation matrix
        public double[] GetMatrix(float[] points, int videoWidth, int videoHeight)
        {
            dstPts.Clear();

            // Create a list of destination points
            dstPts.Add(new Vector2(points[0] / videoWidth, points[1] / videoHeight));
            dstPts.Add(new Vector2(points[2] / videoWidth, points[3] / videoHeight));
            dstPts.Add(new Vector2(points[4] / videoWidth, points[5] / videoHeight));
            dstPts.Add(new Vector2(points[6] / videoWidth, points[7] / videoHeight));

            // Run perspective transform passing source and destination points
            // and set the transformation matrix in the shader
            return GenerateMatrix(srcPts, dstPts);
        }

        // Performs the perspective transform algorithm
        private double[] GenerateMatrix(List<Vector2> src, List<Vector2> dst)
        {
            double[,] A = new double[8, 8];
            double[] B = new double[8];
            double[] X;

            // Algorithm to populate the A and B arrays is based on the OpenCV library getPerspectiveTransform code from (https://github.com/opencv/opencv/blob/4.x/modules/imgproc/src/imgwarp.cpp)
            for (int i = 0; i < 4; i++)
            {
                A[i, 0] = A[i + 4, 3] = src[i].x;
                A[i, 1] = A[i + 4, 4] = src[i].y;
                A[i, 2] = A[i + 4, 5] = 1;
                A[i, 3] = A[i, 4] = A[i, 5] = A[i + 4, 0] = A[i + 4, 1] = A[i + 4, 2] = 0;
                A[i, 6] = -src[i].x * dst[i].x;
                A[i, 7] = -src[i].y * dst[i].x;
                A[i + 4, 6] = -src[i].x * dst[i].y;
                A[i + 4, 7] = -src[i].y * dst[i].y;
                B[i] = dst[i].x;
                B[i + 4] = dst[i].y;
            }

            // Perform Gaussian Elimination
            X = GaussianElimination(A, B);

            return X;
        }

        // Performs the Gaussian Elimination algorithm
        private double[] GaussianElimination(double[,] A, double[] B)
        {
            int N = B.Length;

            for (int p = 0; p < N; p++)
            {
                // find pivot row and swap
                int max = p;
                for (int i = p + 1; i < N; i++)
                {
                    if (Math.Abs(A[i, p]) > Math.Abs(A[max, p]))
                    {
                        max = i;
                    }
                }
                double[] temp = new double[N];
                for (int i = 0; i < N; i++)
                {
                    temp[i] = A[p, i];
                    A[p, i] = A[max, i];
                    A[max, i] = temp[i];
                }
                double t = B[p];
                B[p] = B[max];
                B[max] = t;

                // singular or nearly singular
                if (Math.Abs(A[p, p]) <= double.Epsilon)
                {
                    throw new Exception("Matrix is singular or nearly singular!");
                }

                // pivot within A and B
                for (int i = p + 1; i < N; i++)
                {
                    double alpha = A[i, p] / A[p, p];
                    B[i] -= alpha * B[p];
                    for (int j = p; j < N; j++)
                    {
                        A[i, j] -= alpha * A[p, j];
                    }
                }
            }

            // back substitution
            double[] X = new double[N];
            for (int i = N - 1; i >= 0; i--)
            {
                double sum = 0.0;
                for (int j = i + 1; j < N; j++)
                {
                    sum += A[i, j] * X[j];
                }
                X[i] = (B[i] - sum) / A[i, i];
            }
            return X;
        }
    }
}
