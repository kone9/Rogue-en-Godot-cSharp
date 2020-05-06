using Godot;
using System;
using System.Collections;

public abstract class MovingObject : KinematicBody2D//esta es una clase global que luego usare para que el personaje y los enemigos hereden de esta
{
    
    public float moveTime = 0.1f;//tiempo en moverse de una casilla a otra
    public float movementSpeed;//velocidad a la que se devera mover un objeto de una casilla a otra para que tarde 0.1 
    private int blokingLayer;//referencia a la capa activa se representa por el Z-index
    public CollisionShape2D BoxCollider;
    public RayCast2D rayo;
    public RigidBody2D rb2D; 
    public GameManager _GameManager;
    public bool enable;//para saber si el objeto puede estar enable y moverse
    public Tween moverConTween;
    public int dimensionSprite = 32;
    public bool colisionoConParedLateral = false;
    
    
    private bool colisiono;
    
    // Called when the node enters the scene tree for the first time.
    /*public override void _Ready()//no puedo implementar override desde player o enemigo por eso muevo todas estas variables del ready a cada script independiente
    
    {
        movementSpeed = 1f / moveTime;//velocidad a la que se movera
        BoxCollider = GetNode<CollisionShape2D>("CollisionShape2D");
        rayo = GetNode<RayCast2D>("RayCast2D");
        moverConTween = GetNode<Tween>("Tween");
        //rb2D = thi;por ahora voy a evitarla
    }*/
    //public bool colisiono;

    protected void SmoothMovementWithTween(Vector2 end)//Esta función va a procesar el movimiento en unity se usa una corrutina,pero en godot puede hacerce lo mismo con un nodo tween
    {
        moverConTween.InterpolateProperty(
            this,//que objeto quiero crear la interpolación osea este mismo
            "Position",//que propiedad quiero interpolar,osea la posición
            this.Position,//posición inicial
            end,//posicion final y multiplico por el tamaño del sprite,para esto tengo que hacer pruebas,pero creo que asi tendria que funcionar
            moveTime,//tiempo para ir de una posición a otra
            Tween.TransitionType.Linear,//tipo de interpolación es Tween.TRANS_LINEAR
            Tween.EaseType.InOut//como termina la transición  Tween.EASE_IN_OUT
            );
        moverConTween.Start();//inicializo el nodo tween
    }
       


    //metodo para mover al jugador o al enemigo devuelve 2 si se movio y la colisión de raycast
    protected bool Move(int xDir, int yDir,RayCast2D hitRaycast)//es privada para cualquier otra clase que no hereda de esta
    {
        Vector2 start = Position;//posicion inicial
        Vector2 end = start + new Vector2(xDir,yDir);//la posición de adonde queremos movernos suma de vectores (0,0) + (1,0) = (1,0)
        
        hitRaycast.CastTo = RaycastDirection(xDir,yDir);//esto determina para adonde va apuntar el raycast
        
        //nunca olvidarse de utilizar forceRAycastUpdate en un raycast,es como un tipo bug correguido para que detecte en el proximo frame la colisión
        hitRaycast.ForceRaycastUpdate();//esto es muy importante para detectar colisiones con raycast,sino lo usamos posiblemente no funcione bien la colisión
        ///////////////////////////////////////////////////////////////////
        
        if(!hitRaycast.IsColliding())//sino esta colisionando
        {
            SmoothMovementWithTween(end);//movemos con interpolación lineal recibe por parametro la ultima posición
            return true;
        }
        
        return false;//no hemos podido movernos
    }



    
    
    
    protected virtual bool AttempMove(int xDir,int yDir)//metodo para internar moverse esto recibe por parametro cuanto en x y cuanto en y lo marcamos como abstracto ya que esto lo haremos en cada personaje
    {
        RayCast2D hit = rayo;//tomo la referencia del nodo que esta en la función ready
        bool canMove = Move(xDir,yDir,hit);////si se puede mover o no
        
        if(canMove) return true;//el personaje tendria que mover por lo tanto salgo de esta función
        
        Node col = (Node)hit.GetCollider();//usando la colisicioń del raycast busco el nodo
        if(col != null && col.IsInGroup("Wall"))//si el nodo colisionado esta en el grupo wall
        {
            //GD.Print("estoy conlisionado con un muro");
            OnCantMoveStaticBody2D((StaticBody2D)hit.GetCollider());//aqui tengo que pasarle el staticbody que estamos colisionando
        }
        
        else if(col != null && col.IsInGroup("OuterWall"))//si esta en el grupo pared
        {
            OncanMovePared();
        }

        else if(col != null && col.IsInGroup("characters"))//si el nodo colisionado esta enel grupo character
        {
            //GD.Print("estoy conlisionado con un personaje");
            OnCantMoveRigidBody2D((KinematicBody2D)hit.GetCollider());//aqui tengo que pasarle el kinematicBody que estamos colisionando)
        }
        
        return canMove;//devuelve verdadero o falso para saber si se movio o no
    }   

    
    //Esto es relacionado al movimiento y al tipo de obstaculo
    protected abstract void OnCantMoveStaticBody2D(StaticBody2D go); //aqui viene el comportamiento luego de no poder moverse,es un meotdoABstracto ya que se va a comportar de diferente manera según sea el personaje o elenemigo
    protected abstract void OnCantMoveRigidBody2D(KinematicBody2D go);
    protected abstract void OncanMovePared();//si colisiono con la pared de los laterales
    protected abstract Vector2 RaycastDirection(int xDir, int Ydir);//como el personaje y el enemigo tienen algunas diferencias tengo que procesar la direccioń del raycast en cada uno por separado
    
    
}
