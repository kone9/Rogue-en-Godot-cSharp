using Godot;
using System;
using System.Collections;

public abstract class MovingObject : KinematicBody2D//esta es una clase global que luego usare para que el personaje y los enemigos hereden de esta
{
    
    public float moveTime = 0.1f;//tiempo en moverse de una casilla a otra
    private float movementSpeed;//velocidad a la que se devera mover un objeto de una casilla a otra para que tarde 0.1 
    private int blokingLayer;//referencia a la capa activa se representa por el Z-index
    public CollisionShape2D BoxCollider;
    public RayCast2D rayo;
    public RigidBody2D rb2D; 

    public Tween moverConTween;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        movementSpeed = 1f / moveTime;//velocidad a la que se movera
        BoxCollider = GetNode<CollisionShape2D>("CollisionShape2D");
        rayo = GetNode<RayCast2D>("RayCast2D");
        moverConTween = GetNode<Tween>("Tween");
        //rb2D = thi;por ahora voy a evitarla
    }

    protected void SmoothMovementWithTween(Vector2 end)//Esta función va a procesar el movimiento en unity se usa una corrutina,pero en godot puede hacerce lo mismo con un nodo tween
    {
        moverConTween.InterpolateProperty(
            this,//que objeto quiero crear la interpolación osea este mismo
            "Position",//que propiedad quiero interpolar,osea la posición
            this.Position,//posición inicial
            end,//posicion final
            0.5f,//tiempo para ir de una posición a otra
            Tween.TransitionType.Linear,//tipo de interpolación es Tween.TRANS_LINEAR
            Tween.EaseType.InOut//como termina la transición  Tween.EASE_IN_OUT
            );
        moverConTween.Start();//inicializo el nodo tween
    }

            

    //metodo para mover al jugador o al enemigo devuelve 2 si se movio y la colisión de raycast
    protected bool Move(int xDir, int Ydir,RayCast2D hitRaycast)//es privada para cualquier otra clase que no hereda de esta
    {
        bool colisiono;//para saber cuando el raycast colisiona
        Vector2 start = Position;//posicion inicial
        Vector2 end = start + new Vector2(xDir,Ydir);//la posición de adonde queremos movernos suma de vectores (0,0) + (1,0) = (1,0)
        hitRaycast.CastTo = new Vector2(xDir,Ydir);//esto determina para adonde va apuntar el raycast,por ahora es para prueba ya que supuestamente recibo por parametro donde tendría que apuntar
        
        //hacemos una raycast entre el punto inicial y final
        //si hay un collider por donde pasa esa linea abremos encontrado ese objeto
        //BoxCollider.Visible = false; //desactivamos el boxcollider para que no choque con nosotros mismos en el resultado
        colisiono = hitRaycast.IsColliding();//esto es para verificar si colisionamos tengo que verificar utilizando la colisión del raycast con un grupo
        //BoxCollider.Visible = true;//una ves hecho el raycast lo volvemos a habilitar
        if(colisiono == false)//sino esta colisionando
        {
            //hacemos el movimiento
            SmoothMovementWithTween(end);//movemos con interpolación lineal recibe por parametro la ultima posición
            return true;
        }
        else//si en la mascara 2 hay 1 personaje o un obstaculo
        {
            return false;//no hemos podido movernos
        }
    }

   
    protected abstract void OnCantMove(KinematicBody2D go); //aqui viene el comportamiento luego de no poder moverse,es un meotdoABstracto ya que se va a comportar de diferente manera según sea el personaje o elenemigo

    protected virtual void AttempMove(int xDir,int yDir)//metodo para internar moverse esto recive por arametro cuanto en x y cuanto en y lo marcamos como abstracto ya que cada esto lo haremos en cada personaje
    {
        RayCast2D hit = rayo;//tomo la referencia del nodo que esta en la función ready
        bool canMove = Move(xDir,yDir,hit);//
        if(canMove) return;//si se movio termino esta función
        var col = hit.IsColliding();//nose si funcione luego lo voy a probar con el personaje
        //sino notificamos que nos hemos encontrado cno cierto objeto que nos bloquea el paso,sin embargo el jugador y el enemigo tienen diferente comportamiento
        OnCantMove((KinematicBody2D)hit.GetCollider());//aqui tengo que pasarle el kinematicBody que estamos colisionando
    }
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }


   
}
