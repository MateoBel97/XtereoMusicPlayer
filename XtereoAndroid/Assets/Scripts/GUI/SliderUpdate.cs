using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/*
    La clase SliderUpdate contiene las funciones del slider de tiempo de 
    reproducción en la interfaz gráfica
     */

public class SliderUpdate : MonoBehaviour
{
    public AudioSource musicSource; // AudioSource de la aplicación
    public Slider musicSlider; // Slider de la interfaz gráfica

    void Start()
    {
        musicSlider.minValue = 0.0f; // El valor mínimo del rango del slider es 0
    }

    // Función que reajusta el valor máximo del slider
    public void UpdateSliderValues()
    {
        // El valor máximo del slider se obtiene a partir de la duración en segundos del audio cargado
        musicSlider.maxValue = musicSource.clip.length;
    }

    void Update()
    {
        // Acutaliza constantemente la posición del slider en función del tiempo transcurrido de reproducción
        float timePlayed = musicSource.time;
        musicSlider.value = timePlayed;
    }
    
    // Función que permite el cambio del tiempo de reproducción al variar la posición del slider
    public void OnClick()
    {
        musicSource.time = musicSlider.value;
    }
}
