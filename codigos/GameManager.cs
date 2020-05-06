using Godot;
using Godot.Collections;
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

    private int level = 0;//cuenta el nivel en el que estamos lo usamos como parametro en setup scene
    //public float levelStarDelay;
    private Label LevelText;//referencia al texto del nivel actual
    private Label FoodText;//referencia a la comida que posee el jugador
    private ColorRect fondoColorUI;//referencia al fondo
    public bool inicioEscenario = true;//para saber si inicio el escenario
    public bool doingSetup;//es para saber si todabia esta preparandose la escena,osea esta la UI que aparece en el comienzo

    private SingletonVariables _SingletonVariables;
    Timer TimerReiniciarJuego;

    Player _MovingObject;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        boardScript = (BoardManager)GetTree().GetNodesInGroup("BoardManager")[0];//como el script esta en el nodo lo busco de esta manera
        LevelText = (Label)GetTree().GetNodesInGroup("LevelText")[0];//referencia al texto que muestra el nivel actual
        FoodText = (Label)GetTree().GetNodesInGroup("FoodText")[0];//referencia al texto que muestra la comida
        fondoColorUI = (ColorRect)GetTree().GetNodesInGroup("fondoColorUI")[0];//referencia al fondo de la UI
        
        _SingletonVariables = GetNode<SingletonVariables>("/root/SingletonVariables");//para acceder al singleton desde el player y cambiar el número del nivel y guardar el puntaje
        level = _SingletonVariables.level;
        TimerReiniciarJuego = (Timer)GetTree().GetNodesInGroup("TimerReiniciarJuego")[0];

        InitGame();//llamo al script que inicia el nivel lo hago desde escena principal
        
        //inicioEscenario = true;
    }

    
    public void InitGame()//inicializa el juego
    {
        //inicioEscenario = true;//verifico que ya se incio el escenario
        doingSetup = true;//cuando inciamos el juego se muestra por pantalla la UI que muestra el nivel actual
        enemies.Clear();//antes de iniciar el nivel vaciamos la lista
        boardScript.SetupScene(level);//llamo a la función que esta en board manager para crear la escena toma como parametro el nivel actual que es la cantidad de enemigos
        LevelText.Text = "Day " + level;//el día más lo que alla en la variable level
        fondoColorUI.Visible = true;//para que se vea el fondo
        FoodText.Visible = false;//que no se vea el texto de comida
    }

    public void HideLevelImage()//para dejar de mostrar la imagen inicial esa que dice day 1 más el fondo 
    {
        fondoColorUI.Visible = false;//que se deje de ver el fondo
        LevelText.Visible = false;//se deja de ver el texto
        FoodText.Visible = true;//muestro el texto de la comida
        doingSetup = false;//puedo mover el personaje
    }

    public void _on_TimerlevelStarDelay_timeout()//cuando termina el tiempo del starDelay OJO MANEJADO POR EL NODO TIMER QUE ESTA EN LA ESCENA
    {
        HideLevelImage();//deja de mostrar la imagen inicial
    }




    public async void GameOver()//posiblemente esto despues voy a tener que correguirlo
    {
        
        //await ToSignal(GetTree().CreateTimer(0.5f), "timeout");//detengo por 1 segundo el flujo del código
        LevelText.Text = "After " + level + " Days\nyou starved";//si es game over cambio el texto
        fondoColorUI.Visible = true;//hago visible el fondo
        LevelText.Visible = true;//hace visible el texto   
        Visible = false;//desactivo el nodo que contiene este script,esto tendria que desactivar el Game manager para saber que es Game over puede que lo haga de otra manera
        doingSetup = true;//si es Game over ya no puedo mover al jugador
        await ToSignal(GetTree().CreateTimer(3.0f), "timeout");//detengo por 1 segundo el flujo del código
        LevelText.Text = "The game reload \nin 5 second";//cambio la frase
        TimerReiniciarJuego.Start();//inicio el timer para reiniciar el juego
    }

    private async void MoveEnemies()//esta función sera como una corrutina la utilizare para mover a los personajes cada cierto tiempo
    {
        enemiesMoving = true;//el enemigo se puede mover
        //await ToSignal(GetTree().CreateTimer(turnDelay),"timeout");//creo un timer que espera el tiempo que tenemos en la variable turnDelay...Esto puede cambiar 
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
        if(playersTurn || enemiesMoving || doingSetup)//si es el turno de jugador o los enemigos estan moviendose se esta inciando la escena con el menu 
        {
            return;//salimos de la función y no movemos el personaje
        }
        MoveEnemies();//caso contrario iniciamos el movimiento de los enemigos nose si funcione el delay
        
    }

    public void AddEnemyToList(KinematicBody2D enemy)//los enemigos se agregan cuanso se crean
    {
        enemies.Add(enemy);//agrega el enemigo a la lista
    }

    public void RandomizeSfx( Array<AudioStreamOGGVorbis> clips,AudioStreamPlayer SfxSource )//Función para ejecutar audio recibe el clip y el nodo
    {
        int randomIndex = (int)GD.RandRange(0,clips.Count);//devuelve un número aleatorio entre los 2 clips que recibio como parametro
        float randomPitch = (float)GD.RandRange(0.9f,1.1f);//para cambiar la recuencía del sonido
        SfxSource.PitchScale = randomPitch;//el sonido tiene una pequeña variación
        SfxSource.Stream = clips[randomIndex];//aleatoriamente elijo una de las pistas de audio
        SfxSource.Play();//doy play al sonido
        
    }
    public void _on_TimerReiniciarJuego_timeout()
    {

        GetTree().ReloadCurrentScene();
        _SingletonVariables.food = 100;//cambio los valores del singleton
        _SingletonVariables.level = 1;//cambio los valores del singleton
    }


}
