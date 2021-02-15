using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    La clase GetChannels permite obtener por separado cada canal L y R de un audio
    estereofónico. Al cargar los archivos de audio en C#, el arreglo que se obtiene es
    unidimensional, con muestras del canal L alternadas una por una con muestras del 
    canal R [L1, R1, L2, R2, ... ,Ln, Rn]. Para el procesamiento de las señales se
    separaron los canales en arreglos distintos. Además, para el método earGoggles se 
    obtiene la señal Side (L - R).
     */

public class GetChannels : MonoBehaviour
{
    int numSamples; // Número de samples de audio cargado
    int sampleRate; // Frecuencia de muestreo del audio cargado
    float[] inputSignal; // Señal de entrada
    float[] leftChannel; // Canal L
    float[] rightChannel; // Canal R
    float[] sideChannel; // Señal Side

    public void SetChannels(AudioClip audioClip)
    {
        // Asignación de valores de número de muestras y frecuencia de muestreo
        numSamples = audioClip.samples;
        sampleRate = audioClip.frequency;

        // Creación de arreglos según el tamaño del archivo
        inputSignal = new float[numSamples * 2];
        leftChannel = new float[numSamples];
        rightChannel = new float[numSamples];
        sideChannel = new float[numSamples];

        // Obtención de los datos de la señal original
        audioClip.GetData(inputSignal, 0);

        // Contador para los canales L, R y S
        int j = 0;
        // Iteración en el número de muestras del audio estereofónico original
        for (int i = 0; i < numSamples * 2; i++)
        {
            if ((i % 2) == 0)
            {
                // Canal L en la muestra j
                leftChannel[j] = inputSignal[i];
            }
            else
            {
                // Canal R en la muestra j
                rightChannel[j] = inputSignal[i];
                // Señal Side en la muestra j
                sideChannel[j] = leftChannel[j] - rightChannel[j];
                j++;
            }
        }
    }

    // Función que retorna el canal L
    public float[] GetLeftChannel()
    {
        return leftChannel;
    }

    // Función que retorna el canal R
    public float[] GetRightChannel()
    {
        return rightChannel;
    }

    // Función que retorna la señal Side
    public float[] GetSideChannel()
    {
        return sideChannel;
    }

    // Función que retorna el audio original
    public float[] GetOriginalSignal()
    {
        return inputSignal;
    }
}
