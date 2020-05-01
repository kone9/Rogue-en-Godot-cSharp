using Godot;
using System;

public class SingletonVariables : Node
{
    public int food = 100;
    public int level = 1;
    
    public AudioStreamPlayer musicGame = new AudioStreamPlayer();
    private AudioStreamSample audioMusic = (AudioStreamSample)GD.Load("res://musica/scavengers_music.wav");//precargo la musica

    private Sprite spr = new Sprite(); 
    private void IniciarMusica()//función para procesar datos y inicar la musica
    {
        //esto evita que al reinicar la escena la musica se reinicie ya que
        //esta clase es un singleton que se ejecuta 1 ves sola y luego
        //queda arriba en la gerarquia de los nodos por lo tanto al reiniciar
        //los nodos de abajo este sigue estando activo
        AddChild(musicGame);//agrego el nodo en el singleton para que solo se inicie 1 ves
        AddChild(spr);
        musicGame.Stream = audioMusic;//agrego la musica que busque desde el script
        musicGame.Autoplay = true;//hago que la musica inicie al comienzo del juego
        musicGame.Playing = true;
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {   
        //estube viendo que en el juego no la musica se
        //reinicia en cada ves que cambia de nivel
        //asi que esto lo dejo comentado,sin embargo es bueno
        //saber que es posible hacerlo de esta manera con el singleton
        //IniciarMusica();//inicio la música desde esta clase singleton.
    }
       



    

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
