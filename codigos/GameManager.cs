using Godot;
using System;

public class GameManager : Node2D
{
    //voy a hacer que este GameManager sea un singleton y que nunca se destruya
    public BoardManager boardScript;//creo una referencia al board manager
    public int playerFoodPoint = 100;//puntos iniciales de comida del jugador
    public bool playersTurn = true;//si es el turno del jugador
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

    public void GameOver()
    {
        Visible = false;//desactivo el nodo que contiene este script,esto tendria que desactivar el Game manager para saber que es Game over puede que lo haga de otra manera
    }
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
