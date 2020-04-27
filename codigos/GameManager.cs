using Godot;
using System;
using System.Collections.Generic;

public class GameManager : Node2D
{
    //voy a hacer que este GameManager sea un singleton y que nunca se destruya
    public BoardManager boardScript;//creo una referencia al board manager
    public int playerFoodPoint = 100;//puntos iniciales de comida del jugador
    public bool playersTurn = true;//si es el turno del jugador
    public float turnDelay = 0.1f;//tiempo que va a tardar cada turno
    private List<KinematicBody2D> enemies = new List<KinematicBody2D>();//aqui voy a guardar los enemigos
    private bool enemiesMoving;//si el enemigo se esta moviendo o no

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        boardScript = (BoardManager)GetTree().GetNodesInGroup("BoardManager")[0];//como el script esta en el nodo lo busco de esta manera
        InitGame();//llamo al script que inicia el nivel
    }
    
    private void InitGame()//inicializa el juego
    {
         enemies.Clear();//antes de iniciar el nivel vaciamos la lista
         boardScript.SetupScene(16);//llamo a la función que esta en board manager para crear la escena toma como parametro el nivel actual que es la cantidad de enemigos
    }

    public void GameOver()//posiblemente esto despues voy a tener que correguirlo
    {
        Visible = false;//desactivo el nodo que contiene este script,esto tendria que desactivar el Game manager para saber que es Game over puede que lo haga de otra manera
    }

    private async void MoveEnemies()//esta función sera como una corrutina la utilizare para mover a los personajes cada cierto tiempo
    {
        enemiesMoving = true;//el enemigo se puede mover
        await ToSignal(GetTree().CreateTimer(turnDelay),"timeout");//creo un timer que espera el tiempo que tenemos en la variable turnDelay...Esto puede cambiar 
        if(enemies.Count == 0)//sino hay enemigos
        {
            await ToSignal(GetTree().CreateTimer(turnDelay),"timeout");//esperara este tiempo antes que pueda volver a moverse
        }
        for(int i=0 ; i<enemies.Count; i++)//obtengo cada enemigo
        {
            (enemies[i] as Enemy).MoveEnemy();//busco la función que ejecuta el moviento del enemigo
            await ToSignal(GetTree().CreateTimer((enemies[i] as Enemy).moveTime),"timeout");//espera hasta que termina de moverse,esto podria hacerse de otra forma pero por ahora lo hago igual que el tutorial
        }
        playersTurn = true;//cuando termino de mover todos los enemigos es el turno del jugador esto lo puedo verificar en el player en el bucle principal
        enemiesMoving = false;//el enemigo ya no puede moverse
    }

    
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(float delta)
    {
        if(playersTurn || enemiesMoving)//si es el turno de jugador o los enemigos estan moviendose
        {
            return;//salimos de la función y no hacemos nada
        }
        MoveEnemies();//caso contrario iniciamos el movimiento de los enemigos nose si funcione el delay
        
    }

    public void AddEnemyToList(KinematicBody2D enemy)//los enemigos se agregan cuanso se crean
    {
        enemies.Add(enemy);//agrega el enemigo a la lista
    }
}
