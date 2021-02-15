using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
    La clase PlayClip es el eje de toda la aplicación. Desde esta clase se carga el audio seleccionado,
    se realiza su procesamiento a partir de los métodos de simulación de crosstalk implementados y se
    controla su reproducción según los diferentes componentes de la interfaz gráfica.
     */

public class PlayClip : MonoBehaviour
{
    public AudioSource musicSource; // AudioSource requerida para reproducir los AudioClips
    public AudioClip loadedClip; // AudioClip del audio original
    public AudioClip gilchrestClip; // AudioClip del audio procesado por el método Nearfield Crosstalk
    public AudioClip manorClip; // AudioClip del audio procesado por el método earGoggles

    public string currentMethod; // Variable que almacena el método de reproducción seleccionado
    
    LoadAudioFiles loader; // Clase LoadAudioFiles implementada en la aplicación
    GilchrestProcessor gilchrest; // Clase GilchrestProcessor implementada en la aplicación
    ManorProcessor manor; // Clase ManorProcessor implementada en la aplicación

    Text canvasText; // Texto de la interfaz gráfica
    SliderUpdate sliderUpdate; // Slider de la interfaz gráfica

    // GameObjects para la selección del método de reproducción (original, Nearfield Crosstalk, earGoggles)
    GameObject originalSelected, manorSelected, gilchrestSelected;

    string currentFileName; // Nombre del archivo reproducido

    void Start()
    {
        // Asignación de la frecuencia de muestreo de salida a 44.1 kHz
        AudioSettings.outputSampleRate = 44100;
        
        // Búsqueda de clase LoadAudioFiles implementada en la aplicación
        loader = GameObject.Find("AudioPlayer").GetComponent<LoadAudioFiles>();
        // Búsqueda de clase ManorProcessor implementada en la aplicación
        manor = GameObject.Find("AudioPlayer").GetComponent<ManorProcessor>();
        // Búsqueda de clase GilchrestProcessor implementada en la aplicación
        gilchrest = GameObject.Find("AudioPlayer").GetComponent<GilchrestProcessor>();

        // Búsqueda del texto de la interfaz gráfica
        canvasText = GameObject.Find("Texto").GetComponent<Text>();
        // Búsqueda del slider de la interfaz gráfica
        sliderUpdate = GameObject.Find("MusicSlider").GetComponent<Slider>().GetComponent<SliderUpdate>();
        // Búsqueda de la barra que indica la selección del método original
        originalSelected = GameObject.Find("OriginalSelected");
        // Búsqueda de la barra que indica la selección del método Nearfield Crosstalk
        manorSelected = GameObject.Find("ManorSelected");
        manorSelected.SetActive(false); // Se desactiva al activar otro método de reproducción
        // Búsqueda de la barra que indica la selección del método earGoggles
        gilchrestSelected = GameObject.Find("GilchrestSelected");
        gilchrestSelected.SetActive(false); // Se desactiva al activar otro método de reproducción

        // Selección del método original para reproducción del audio
        currentMethod = "original";

    }

    // Función que se activa al cambiar de métod de reproducción
    public void ChangeMethod(string newMethod)
    {
        if(newMethod != currentMethod) // Si se cambió de método de reproducción...
        {   
            musicSource.Pause(); // El audio se pausa
            float timePlayed = musicSource.time; // El tiempo reproducido (en segundos) es guardado 
            currentMethod = newMethod; // Se cambia el método de reproducción
            // Asignación del método seleccionado. Al activar uno, se desactivan los otros.
            // Se asigna al AudioSource de la aplicación el AudioClip respectivo al método seleccionado
            switch (currentMethod)
            {
                case "original":
                    musicSource.clip = loadedClip;
                    originalSelected.SetActive(true);
                    manorSelected.SetActive(false);
                    gilchrestSelected.SetActive(false);
                    break;
                case "manor":
                    musicSource.clip = manorClip;
                    originalSelected.SetActive(false);
                    manorSelected.SetActive(true);
                    gilchrestSelected.SetActive(false);
                    break;
                case "gilchrest":
                    musicSource.clip = gilchrestClip;
                    originalSelected.SetActive(false);
                    manorSelected.SetActive(false);
                    gilchrestSelected.SetActive(true);
                    break;
            }
            musicSource.time = timePlayed; // Se reasigna el tiempo de reproducción transcurrido
            musicSource.Play(); // El audio seleccionado es reproducido
        }
    }

    // Función llamada por la clase LoadAudioFiles al cargar exitosamente el archivo de audio
    public void AudioLoaded()
    {
        if (loadedClip != null)
        {
            // Procesamiento del AudioClip del archivo seleccionado mediante el método Nearfield Crosstalk
            manorClip = manor.Process(loadedClip);
            // Procesamiento del AudioClip del archivo seleccionado mediante el método earGoggles
            gilchrestClip = gilchrest.Process(loadedClip);
            // Asignación del AudioClip según el método de reproducción seleccionado
            switch (currentMethod)
            {
                case "gilchrest":
                    musicSource.clip = gilchrestClip;
                    break;
                case "manor":

                    musicSource.clip = manorClip;
                    break;
                case "original":
                    musicSource.clip = loadedClip;
                    break;
            }
            // Se reinicia el tiempo de reproducción
            musicSource.time = 0.0f;
            // Los rangos del Slider son actualizados
            sliderUpdate.UpdateSliderValues();
            // Reproducción del AudioClip seleccionado
            musicSource.Play();
            // Asignación del título del archivo WAV al texto de la interfaz gráfica
            canvasText.text = currentFileName;
        }
    }

    // Función que se activa al oprimir alguno de los botones dentro del SrcollView
    public void PlayAudio(string chosenFilePath, string fileName)
    {
        // Se detiene la reproducción
        if (musicSource.isPlaying) { 
            musicSource.Stop();
        }

        canvasText.text = "Cargando...";

        // Destrucción de los AudioClips para liberar memoria
        DestroyClips();

        // Activación de la función LoadClip de la clase LoadAudioFiles
        loader.LoadClip(chosenFilePath);

        currentFileName = fileName;
        
    }

    // Destrucción de los AudioClips para liberar memoria
    void DestroyClips()
    {
        AudioClip.Destroy(loadedClip);
        AudioClip.Destroy(gilchrestClip);
        AudioClip.Destroy(manorClip);
        gilchrest.DestroyClip();
        manor.DestroyClip();
    }

    // Función para pausar reproducción
    public void Pause()
    {
        if (musicSource.isPlaying) { 
            musicSource.Pause();
        }
    }

    // Función para reanudar reproducción
    public void Play()
    {
        if (!musicSource.isPlaying)
        { 
            musicSource.Play(0);
        }
    }
}
