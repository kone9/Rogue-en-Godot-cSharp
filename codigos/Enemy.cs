using Godot;
using Godot.Collections;
using System;

public class Enemy : MovingObject
{
   
    [Export]
    public int playerDamage = 10;//el daño que le hace el enemigo al jugador al golpearlo
    private KinematicBody2D targetCharacter;
    private Vector2 targetPosition;//referencia a la posición donde estara el jugador
    private bool skinMove;//como es por turnos esta variable es para saber cuando puede moverse
    private AnimationNodeStateMachinePlayback playback;//referencia al animator

    private Array<AudioStreamOGGVorbis> scavengers_enemy = new Array<AudioStreamOGGVorbis>();//cargo en este arreglo el recurso de audio enemigo,nota importante el arreglo de Godot es un Generico
    private AudioStreamPlayer scavengersEnemyNode;//referencia al nodo
    Player hitPlayer;//la inicio vacia

    private AnimationPlayer AnimationPlayerEfectos;//referencia al animation player

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        //para procesar el game manager
        _GameManager = (GameManager)GetTree().GetNodesInGroup("GameManager")[0];//para poder acceder al GameManager
        _GameManager.AddEnemyToList(this);//tendria que agregarse este mismo objeto kinematico2D

        
        //para procesar el enemigo
        movementSpeed = 1f / moveTime;//velocidad a la que se movera
        rayo = GetNode<RayCast2D>("RayCast2D");//referencia al raycast
        moverConTween = GetNode<Tween>("Tween");//referencia al nodo tween
        targetCharacter = (KinematicBody2D)GetTree().GetNodesInGroup("Player")[0];//referencia al nodo del personaje
        playback = (AnimationNodeStateMachinePlayback)GetNode<AnimationTree>("AnimationTree").Get("parameters/playback");//accedo al nodo animation three y a la propiedad de las maquinas de estado
        playback.Start("EnemyIdle");//como inicia la animación del personaje
        
        //para procesar el audio
        scavengersEnemyNode = (AudioStreamPlayer)GetTree().GetNodesInGroup("scavengers_enemy")[0];//busco el nodo que controla el audio
        scavengers_enemy.Add((AudioStreamOGGVorbis)GD.Load("res://musica/scavengers_enemy1.ogg"));//agrego el recurso de sonido
        scavengers_enemy.Add((AudioStreamOGGVorbis)GD.Load("res://musica/scavengers_enemy2.ogg"));//agrego el recurso de sonido

        //procesar animaciones de efectos
        AnimationPlayerEfectos = (AnimationPlayer)GetTree().GetNodesInGroup("AnimationPlayerEfectos")[0];
    }

    public override void _Process(float delta)
    {
        targetPosition = targetCharacter.Position;//guardo la posición del personaje en targetPosition
    }


    protected override bool AttempMove(int xDir, int yDir)//este metodo devuelve si se movio o no "Bool"
    {
        
        if(skinMove)//si se movio
        {
            skinMove = false;//desactivo el movimiento
            return false;//retorna falso,el enemigo no se a movido
        }
        bool canMove = base.AttempMove(xDir,yDir);//la segunda ves sera falso y se ejecutara el movimiento
        skinMove = true;//una ves hecho el movimiento otra ves sera verdadero
        return canMove;//regresa si se movio o no
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
        GD.Print("enemigo colisionando con alguien");
        //parece que hay un bug cuando colisionan entre enemigos por eso verifico si esta en el grupo player el cuerpo kinematico
        if(go.IsInGroup("Player"))//detectamos que esta en el grupo player para evitar un bug al solicionar los enemigos entre sí
        {
            hitPlayer = (Player)go;//de esta forma tendria que acceder al script que tiene el personaje
        }

        if(hitPlayer != null)//si estoy en la casilla player
        {
            hitPlayer.LoseFood(playerDamage);//descuento comida del personaje
            
            playback.Travel("EnemyAttack");//el enemigo ataca cuando le quitamos puntos al jugador
            _GameManager.RandomizeSfx(scavengers_enemy,scavengersEnemyNode);//activo sonido golpe
            if(hitPlayer.food >= 10)//si la comida del personaje es mayor o igual a 10
            {
                AnimationPlayerEfectos.Play("pantallaRoja");//play color rojo en pantalla
                AnimationPlayerEfectos.PlayBackwards("pantallaRoja");//play al reves color rojo en pantalla para que quede transparente
            }
        }
    }


    protected override void OncanMovePared()//si el enemigo choca contra la pared no hace nada
    {
        return;//salgo de la función
    }
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
