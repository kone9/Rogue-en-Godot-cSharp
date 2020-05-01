using Godot;
using Godot.Collections;
using System;

public class Wall : Sprite
{
    [Export]
    private int indiceDmgSprite = 0;
    public int hp = 4;
    private CollisionShape2D _collisionShape2D;
    private GameManager _GameManager;

    //para procesar el sonido
    private Array<AudioStreamOGGVorbis> scavengers_chop = new Array<AudioStreamOGGVorbis>();//cargo en este arreglo el recurso de audio chop,nota importante el arreglo de Godot es un Generico
    private AudioStreamPlayer scavengersChopNode;//referencia al nodo

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _collisionShape2D = GetNode<CollisionShape2D>("StaticBody2DWall/CollisionShape2D");
        _GameManager = (GameManager)GetTree().GetNodesInGroup("GameManager")[0];
        scavengersChopNode = (AudioStreamPlayer)GetTree().GetNodesInGroup("scavengers_chop")[0];
        scavengers_chop.Add((AudioStreamOGGVorbis)GD.Load("res://musica/scavengers_chop1.ogg"));//guardo el audio precargado    
        scavengers_chop.Add((AudioStreamOGGVorbis)GD.Load("res://musica/scavengers_chop2.ogg"));//guardo el audio precargado    
    }


    public void DamageWall(int loss)//funcion publica que usara el personaje para dañar y hacer invisible este nodo
    {
        //aqui viene el sonido de golpear la columna
        _GameManager.RandomizeSfx(scavengers_chop,scavengersChopNode);//activo sonido golpear
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
