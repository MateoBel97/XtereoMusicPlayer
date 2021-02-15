using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    La clase Filter permite la implementación de la ecuación de entrada y salida de
    los filtro digitales para realizar el procesamiento de señal.
     */

public class Filter : MonoBehaviour
{
    /* Función que recibe un arreglo con muestras de audio y la procesa a partir de los 
       coeficientes de entrada y salida que se le ingresen.
       */
    public float[] FilterSignal(float[] signal, float[] bCoefs, float[] aCoefs)
    {
        // Creación de la señal de salida con igual tamaño que la señal de entrada
        float[] filteredSignal = new float[signal.Length];
        // Creación de arreglos para valores de entrada en muestras anteriores
        float[] x = new float[bCoefs.Length];
        // Creación de arreglos para valores de salida en muestras anteriores
        float[] y = new float[aCoefs.Length];

        // Iteración en el número de muestras de la señal de entrada
        for (int i = 0; i < signal.Length; i++)
        {
            float sum = 0; // Valor de cada muestra calculada

            // Reasignación de los valores de entrada previos
            for (int ix = (bCoefs.Length - 1); ix > 0; ix--)
            {
                x[ix] = x[ix - 1];
            }
            x[0] = signal[i];

            // Reasignación de los valores de salida previos
            for (int iy = (aCoefs.Length - 1); iy > 0; iy--)
            {
                y[iy] = y[iy - 1];
            }

            // Sumatoria de valores de entrada previos por sus coeficientes de realimentación correspondientes
            for (int b = 0; b < bCoefs.Length; b++)
            {
                sum += bCoefs[b] * x[b];
            }

            // Sumatoria de valores de salida previos por sus coeficientes de retroalimentación correspondientes
            for (int a = 1; a < aCoefs.Length; a++)
            {
                sum -= aCoefs[a] * y[a];
            }
            y[0] = sum;

            // Asignación de valor calculado a la señal de salida
            filteredSignal[i] = sum;
        }
        return filteredSignal;
    }
}
