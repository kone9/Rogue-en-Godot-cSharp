using Godot;
using System;

public class Wall : Sprite
{
    [Export]
    private int indiceDmgSprite = 0;
    public int hp = 4;
    private CollisionShape2D _collisionShape2D;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _collisionShape2D = GetNode<CollisionShape2D>("StaticBody2DWall/CollisionShape2D");
    }

    public void DamageWall(int loss)//funcion publica que usara el personaje para dañar y hacer invisible este nodo
    {
        Frame = indiceDmgSprite;//cuando es golpeado toma esta textura que esta en el indice DmgSprite
        hp -= loss;//descuento puntaje
        if(hp<2)
        if(hp <= 0)//si el hp de esta columna es menor a cero
        {
            _collisionShape2D.Disabled = true;//hago invisible el cuerpo estatico ya que sigue funcionando aunque haga invisible el padre
            Visible = false;//esto tengo que comprobar ya que a veces los hijos si son areas o bodys siguen funciondo
            //GD.Print("tendria que dejar de estar colisionando con el la pared");
            //QueueFree();//pruebo destruyendo,aunque tengo que hacerlo desaparecer ya que dicen que si destruyo los objetos tienen consumo con el recolector de basura de c#
        }
    }
}
