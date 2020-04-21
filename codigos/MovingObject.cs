using Godot;
using System;

public class MovingObject : Node
{
    
    public float moveTime = 0.1f;//tiempo en moverse de una casilla a otra
    private float movementSpeed;//velocidad a la que se devera mover un objeto de una casilla a otra para que tarde 0.1 
    private int blokingLayer;//referencia a la capa activa se representa por el Z-index
    private CollisionShape2D BoxCollider;
    private RigidBody2D rb2D; 

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        movementSpeed = 1f / moveTime;//velocidad a la que se movera
        BoxCollider = (CollisionShape2D)GetTree().GetNodesInGroup("CollisionShape2D")[0];
        //rb2D = thi;por ahora voy a evitarla
    }

    //metodo para mover al jugador o al enemigo devuelve 2 valores el raycast y si se movio
    protected bool Move(int xDir, int Ydir,RayCast2D hit)//es privada para cualquier otra clase que no hereda de esta
    {
        Vector2 start = Position2D;
    }
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
