using Godot;
using System;

public class GameManager : Node
{
    //voy a hacer que este GameManager sea un singleton y que nunca se destruya
    public BoardManager boardScript;//creo una referencia al board manager

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        boardScript = (BoardManager)GetTree().GetNodesInGroup("BoardManager")[0];//como el script esta en el nodo lo busco de esta manera
        InitGame();//llamo al script que inicia el nivel
    }
    
    private void InitGame()//inicializa el juego
    {
         boardScript.SetupScene(16);//llamo a la función que esta en board manager para crear la escena toma como parametro el nivel actual que es la cantidad de enemigos
    }


//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
