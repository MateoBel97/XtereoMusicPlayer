using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
    La clase ManoProcessor permite procesar un AudioClip mediante el método
    de Nearfield Crosstalk para simulación de crosstalk en audífonos. Es llamada por la clase
    PlayClip a la hora de cargar el AudioClip del archivo WAV seleccionado en el
    ScrollView.
 */
public class ManorProcessor : MonoBehaviour
{
    GetChannels gc; // Objeto GetChannels para obtención de canales L, R y S
    Filter filter; // Objeto filter para procesamiento digital de señales
    AudioClip processedClip; // AudioClip que almacene el resultado del método earGoggles

    // Arreglos para los coeficientes de realimentación y retroalimentación para el filtro High Shelving
    float[] bCoefsShelving = new float[2], aCoefsShelving = new float[2];
    // Arreglos para los coeficientes de realimentación y retroalimentación para el filtro Low Pass
    float[] bCoefsLowPass = new float[2], aCoefsLowPass = new float[2];

    // Arreglos para los canales Left y Right originales 
    float[] leftChannel, rightChannel;
    // Arreglos para los canales Left y Righ procesados por el filtro High Shelving
    float[] leftShelving, rightShelving;
    // Arreglos para los canales Left y Righ procesados por el filtro Low Pass
    float[] leftLowPass, rightLowPass;

    void Start()
    {
        // Búsqueda de la clase GetChannels implementada en el GameObject AudioPlayer
        gc = GameObject.Find("AudioPlayer").GetComponent<GetChannels>();
        // Búsqueda de la clase Filter implementada en el GameObject AudioPlayer
        filter = GameObject.Find("AudioPlayer").GetComponent<Filter>();
    }

    public AudioClip Process(AudioClip clipToProcess)
    {
        // Arreglo que contiene la señal de audio procesada
        float[] processedSignal = new float[clipToProcess.samples * 2];
        // Creación del AudioClip que contiene el audio procesado por el método earGoggles
        processedClip = AudioClip.Create("Processed", clipToProcess.samples, clipToProcess.channels, clipToProcess.frequency, false);
        // Obtención de frecuencia de muestreo del audio original
        int sampleRate = clipToProcess.frequency;
        // Activación de la función SetChannels de la clase GetChannels
        gc.SetChannels(clipToProcess);
        /*  Asignación de los valores para los coeficientes de los filtros según
            la frecuencia de muestreo del audio original. Está restringida para
            archivos muestreados a 44.1 kHz.
         */
        SetFilterCoefs(sampleRate);

        leftChannel = gc.GetLeftChannel(); // Obtención del canal L
        rightChannel = gc.GetRightChannel(); // Obtención del canal R

        // Procesamiento del canal L mediante el filtro High Shelving
        leftShelving = filter.FilterSignal(leftChannel, bCoefsShelving, aCoefsShelving);
        // Procesamiento del canal R mediante el filtro High Shelving
        rightShelving = filter.FilterSignal(rightChannel, bCoefsShelving, aCoefsShelving);
        // Procesamiento del canal L mediante el filtro Low Pass
        leftLowPass = filter.FilterSignal(leftChannel, bCoefsLowPass, aCoefsLowPass);
        // Procesamiento del canal R mediante el filtro Low Pass
        rightLowPass = filter.FilterSignal(rightChannel, bCoefsLowPass, aCoefsLowPass);

        // Cálculo del delay para la señal Side filtrada (0.5 ms)
        float delayLowPass = 0.0005f;
        int sampleDelayLowPass = (int)(sampleRate * delayLowPass);
        float gain = 1f; // Ganancia del audio

        int j = 0;// Contador que itera entre cada muestra de los canales originales y procesados
        /*  Iteración para el cálculo de cada muestra de la señal procesada.
            Al ser estereofónico, las muestras del audio van intercaladas para los canales L y R    
         */
        
        for (int i = 0; i < clipToProcess.samples * 2; i++)
        {
            float val = 0f; // Variable que almacena el valor de cada muestra de audio
            if (i%2 == 0) // Si el contador i es par, se obtiene el valor de la señal para el canal L
            {
                // Suma del canal L procesado por el filtro High Shelving
                val += leftShelving[j] * gain;
                if (j >= sampleDelayLowPass)
                {
                    // Suma del canal R procesado por el filtro Low Pass con un retraso de 0.5 ms
                    val += rightLowPass[j - sampleDelayLowPass] * gain;
                }
            }
            else // Si el contador i es impar, se obtiene el valor de la señal para el canal R
            {
                // Suma del canal R procesado por el filtro High Shelving
                val += rightShelving[j] * gain;
                if(j >= sampleDelayLowPass)
                {
                    // Suma del canal L procesado por el filtro Low Pass con un retraso de 0.5 ms
                    val += leftLowPass[j - sampleDelayLowPass] * gain;  
                }
                j++;
            }
            // Asignación del valor calculado en el arreglo del audio procesado por el método Nearfield Crosstalk0
            processedSignal[i] = val;
        }
        // Asignación de la señal procesada al AudioClip
        processedClip.SetData(processedSignal, 0);
        // Eliminación de los canales obtenido para liberar memoria
        leftChannel = null;
        rightChannel = null;
        leftShelving = null;
        rightShelving = null;
        leftLowPass = null;
        rightLowPass = null;
        processedSignal = null;

        return processedClip;
    }

    public void DestroyClip() // Eliminación del AudioClip obtenido para liberar memoria
    {
        AudioClip.Destroy(processedClip);
        
        
    }
    
    public void SetFilterCoefs(int sampleRate)  // Asignación de coeficientes para los filtros
    {
        switch (sampleRate)
        {
            case 44100:
                bCoefsShelving[0] = 0.9792f;
                bCoefsShelving[1] = -0.8851f;
                aCoefsShelving[0] = 1.0f;
                aCoefsShelving[1] = -0.8668f;

                bCoefsLowPass[0] = 0.0383f;
                bCoefsLowPass[1] = 0.0383f;
                aCoefsLowPass[0] = 1.0f;
                aCoefsLowPass[1] = -0.784f;
                break;
        }
    }

}
