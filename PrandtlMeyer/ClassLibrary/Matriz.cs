using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;



namespace ClassLibrary
{
    public class Matriz
    {
        public Matriz() { }

        Cell[,] matrix;

        public void actualizarRectanglesMatrix()
        {
            double AX = 0;

            for (int i = 0; AX < 10; i++)
            {
                for (int j = 0; j < 40; j++)
                {
                    matrix[i, j].calcularRectangle(40, 2);
                }
                AX += matrix[i, 1].rectangle.Width;
            }

        }

    }
}
