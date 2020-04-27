using Godot;
using System;

public class Enemy : MovingObject
{
   
    [Export]
    public int playerDamage = 10;//el daño que le hace el enemigo al jugador al golpearlo
    private KinematicBody2D targetCharacter;
    private Vector2 targetPosition;//referencia a la posición donde estara el jugador
    private bool skinMove;//como es por turnos esta variable es para saber cuando puede moverse
    private AnimationNodeStateMachinePlayback playback;//referencia al animator

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _GameManager = (GameManager)GetTree().GetNodesInGroup("GameManager")[0];//para poder acceder al GameManager
        _GameManager.AddEnemyToList(this);//tendria que agregarse este mismo objeto kinematico2D

        movementSpeed = 1f / moveTime;//velocidad a la que se movera
        rayo = GetNode<RayCast2D>("RayCast2D");//referencia al raycast
        moverConTween = GetNode<Tween>("Tween");//referencia al nodo tween
        targetCharacter = (KinematicBody2D)GetTree().GetNodesInGroup("Player")[0];//referencia al nodo del personaje
        playback = (AnimationNodeStateMachinePlayback)GetNode<AnimationTree>("AnimationTree").Get("parameters/playback");//accedo al nodo animation three y a la propiedad de las maquinas de estado
        playback.Start("EnemyIdle");//como inicia la animación del personaje
    }

    public override void _Process(float delta)
    {
        targetPosition = targetCharacter.Position;//guardo la posición del personaje en targetPosition
    }

    protected override void AttempMove(int xDir, int yDir)
    {
        
        if(skinMove)//si se movio
        {
            skinMove = false;//desactivo el movimiento
            return;//salgo de esta función
        }
        base.AttempMove(xDir,yDir);//la segunda ves sera falso y se ejecutara el movimiento
        skinMove = true;//una ves hecho el movimiento otra ves sera verdadero
    }

    public void MoveEnemy()//metodo que se encarga de mover al personaje,esto es publico y lo hara el GameManager en una lista de enemigos
    {
        int xDir = 0;//posición donde nos vamos a mover
        int yDir = 0;//posición donde nos vamos a mover
        //si el jugador esta alineado verticalmente con el enemigo
        //el enemigo se mueve de arriba abajo "Vertical"
        //sino estan alineados el enemigo se mueve izquierda derecha "Horizontal"
        if(Math.Abs(targetPosition.x - Position.x) < float.Epsilon)//si el jugador esta en la misma posición en X que el enemigo usa float epsilon para determinar un valor muy pequeño y el valor absoluto es para que siempre sea positivo o eso creo
        {
            //si la posición en y del target el mayor a la posición en y de enemigo
            //el enemigo se mueve hacia arriba,sino se mueve hacia abajo
            yDir = targetPosition.y > Position.y ? 1 * dimensionSprite : -1 * dimensionSprite;//siempre multiplico por el tamaño de sprite 
        }
        else //sino el jugador no esta en la mismo posición en Y que el enemigo entonces el enemigo se mueve en el eje X
        {
             //si la posición en x del target el mayor a la posición en x de enemigo
            //el enemigo se mueve hacia izquierdo,sino se mueve hacia derecha//Esto puede ser al reves
            xDir = targetPosition.x > Position.x ? 1 * dimensionSprite : -1 * dimensionSprite;
        }
        AttempMove(xDir,yDir);
            
    }

    protected override void OnCantMoveStaticBody2D(StaticBody2D go)//sino puede moverse por un cuerpo statico osea esto seria los obstaculos que estan en medio del escenario
    {
        
    }
      
    protected override Vector2 RaycastDirection(int xDir, int Ydir)//cuando el raycast es del enemigo
    {
        return new Vector2(xDir,Ydir);//devuelvo la posición normal y con esto funciona bien hacia adonde apunta el raycast
    }
    
    protected override void OnCantMoveRigidBody2D(KinematicBody2D go)//sino puede moverse por un kinematicbody como un personaje
    {
        Player hitPlayer = (Player)go;//de esta forma tendria que acceder al script que tiene el personaje
        if(hitPlayer != null)//si estoy en la casilla player
        {
            hitPlayer.LoseFood(playerDamage);//descuento comida del personaje
            playback.Travel("EnemyAttack");//el enemigo ataca cuando le quitamos puntos al jugador
        }
    }
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
