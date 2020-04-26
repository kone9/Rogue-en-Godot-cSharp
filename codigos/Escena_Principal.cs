using Godot;
using System;

public class Escena_Principal : Node2D
{
    [Export]
    PackedScene GameManager;
    private bool existeGameManager = false;
    //Node2D escenaNueva;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        TestGameManager();
    }
            


    private void TestGameManager() //esto es para verificar si existe en el nodo un game manager,caso contrario instancio 1
    {
        //esto es para verificar si existe en el nodo un game manager,caso contrario instancio 1
        foreach(Node i in GetTree().GetNodesInGroup("GameManager"))//recorro el grupo de nodos que estan en este grupo
        {
            if(i.IsInsideTree())//si esta en la escena
            {
                existeGameManager = true;//existe un game Manager
                //GD.Print("ya existe un gameManger");
            }
        }
        if(existeGameManager == false)//sino existe un game manager
        {
            //GD.Print("No existe un GameManger creo uno nuevo");
            instanciarGameManager();//instancio el gameManager
        }
    }
    
    
    private void instanciarGameManager()
    {
        var escenaNueva = (Node)GameManager.Instance();//creo una instancia de la escea empaqueda
        AddChild(escenaNueva);//hago que sea hijo de este nodo 
    }


//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
