using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
    La clase GilchrestProcessor permite procesar un AudioClip mediante el método
    de earGoggles para simulación de crosstalk en audífonos. Es llamada por la clase
    PlayClip a la hora de cargar el AudioClip del archivo WAV seleccionado en el
    ScrollView.
 */
public class GilchrestProcessor : MonoBehaviour
{
    GetChannels gc; // Objeto GetChannels para obtención de canales L, R y S
    Filter filter; // Objeto filter para procesamiento digital de señales
    AudioClip processedClip; // AudioClip que almacene el resultado del método earGoggles

    // Arreglos para los coeficientes de realimentación y retroalimentación para el filtro Head Model
    float[] bCoefsHeadModel = new float[2], aCoefsHeadModel = new float[2];
    // Arreglos para los coeficientes de realimentación y retroalimentación para el filtro Low Pass
    float[] bCoefsLowPass = new float[3], aCoefsLowPass = new float[3];
    // Arreglos para los coeficientes de realimentación y retroalimentación para el filtro High Pass
    float[] bCoefsHighPass = new float[3], aCoefsHighPass = new float[3];

    // Arreglos para los canales Left, Righ y Side del audio original.
    float[] leftChannel, rightChannel, sideChannel;
    // Arreglos para los canales Left y Righ procesados por el filtro LowPass
    float[] leftLowPass, rightLowPass;
    // Arreglos para la señal Side procesada por los filtros Head Model y High Pass
    float[] sideHeadModel, sideHighPass;

    void Start()
    {
        // Búsqueda de la clase GetChannels implementada en el GameObject AudioPlayer
        gc = GameObject.Find("AudioPlayer").GetComponent<GetChannels>();
        // Búsqueda de la clase Filter implementada en el GameObject AudioPlayer
        filter = GameObject.Find("AudioPlayer").GetComponent<Filter>();
    }

    /*  Función que recibe el AudioClip del audio original y retorna
        el AudioClip procesado por él método earGoggles
    */
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
        sideChannel = gc.GetSideChannel(); // Obtención de la señal Side

        // Procesamiento del canal L mediante el filtro Low Pass
        leftLowPass = filter.FilterSignal(leftChannel, bCoefsLowPass, aCoefsLowPass);
        // Procesamiento del canal R mediante el filtro Low Pass
        rightLowPass = filter.FilterSignal(rightChannel, bCoefsLowPass, aCoefsLowPass);
        // Procesamiento de la señal Side mediante el filtro Head Model
        sideHeadModel = filter.FilterSignal(sideChannel, bCoefsHeadModel, aCoefsHeadModel);
        // Procesamiento de la señal Side mediante el filtro High Pass
        sideHighPass = filter.FilterSignal(sideChannel, bCoefsHighPass, aCoefsHighPass);

        // Cálculo del delay para la señal Side filtrada (15 ms)
        float delayHighPass = 0.015f;
        int sampleDelayHighPass = (int)(sampleRate * delayHighPass);
        // Cálculo del delay para los canales L y R procesados por el filtro Low Pass (20 ms)
        float delayLowPass = 0.020f;
        int sampleDelayLowPass = (int)(sampleRate * delayLowPass);

        int j = 0; // Contador que itera entre cada muestra de los canales originales y procesados
        /*  Iteración para el cálculo de cada muestra de la señal procesada.
            Al ser estereofónico, las muestras del audio van intercaladas para los canales L y R    
         */
        for (int i = 0; i < clipToProcess.samples * 2; i++)
        {
            float val = 0f; // Variable que almacena el valor de cada muestra de audio
            if ((i % 2) == 0) // Si el contador i es par, se obtiene el valor de la señal para el canal L
            {
                // Suma del canal L original
                val += leftChannel[j];
                // Suma de la señal Side procesada por el filtro Head Model
                val += sideHeadModel[j];
                if (j >= sampleDelayHighPass)
                {
                    // Suma de la señal Side procesada por el filtro High Pass con un retraso de 15 ms
                    val += sideHighPass[j - sampleDelayHighPass]; 
                }
                if (j >= sampleDelayLowPass)
                {
                    // Suma del canal R procesado por el filtro Low Pass con un retraso de 20 ms
                    val += rightLowPass[j - sampleDelayLowPass];
                }

            }
            else // Si el contador i es impar, se obtiene el valor de la señal para el canal R
            {
                // Suma del canal R original
                val += rightChannel[j];
                // Suma de la señal Side invertida y procesada por el filtro Head Model
                val -= sideHeadModel[j];
                if (j >= sampleDelayHighPass)
                {
                    // Suma de la señal Side invertida y 
                    // procesada por el filtro High Pass con un retraso de 15 ms
                    val -= sideHighPass[j - sampleDelayHighPass];
                }
                if (j >= sampleDelayLowPass)
                {
                    // Suma del canal L procesado por el filtro Low Pass con un retraso de 20 ms
                    val += leftLowPass[j - sampleDelayLowPass];
                }
                j++;
            }
            // Asignación del valor calculado en el arreglo del audio procesado por el método earGoggles
            processedSignal[i] = val * 0.7f;
        }
        // Asignación de la señal procesada al AudioClip
        processedClip.SetData(processedSignal, 0);
        // Eliminación de los canales obtenido para liberar memoria
        leftChannel = null;
        rightChannel = null;
        sideChannel = null;
        leftLowPass = null;
        rightLowPass = null;
        sideHighPass = null;
        sideHeadModel = null;
        processedSignal = null;

        return processedClip;
    }

    public void DestroyClip() // Eliminación del AudioClip obtenido para liberar memoria
    {
        AudioClip.Destroy(processedClip);
    }

    public void SetFilterCoefs(int sampleRate) // Asignación de coeficientes para los filtros
    {
        switch (sampleRate)
        {
            case 44100:
                bCoefsHeadModel[0] = 0.3023f;
                bCoefsHeadModel[1] = -0.1293f;
                aCoefsHeadModel[0] = 1.0f;
                aCoefsHeadModel[1] = -0.827f;

                bCoefsLowPass[0] = 0.0001546f / 2f;
                bCoefsLowPass[1] = 0.0003093f / 2f;
                bCoefsLowPass[2] = 0.0001546f / 2f;
                aCoefsLowPass[0] = 1.0f;
                aCoefsLowPass[1] = -1.9496f;
                aCoefsLowPass[2] = 0.9509f;

                bCoefsHighPass[0] = 0.3066f;
                bCoefsHighPass[1] = -0.6132f;
                bCoefsHighPass[2] = 0.3066f;
                aCoefsHighPass[0] = 1.0f;
                aCoefsHighPass[1] = -1.0709f;
                aCoefsHighPass[2] = 0.3821f;
                break;
        }
    }

    

}
