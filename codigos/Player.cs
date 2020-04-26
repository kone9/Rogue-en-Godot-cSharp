using Godot;
using System;

public class Player : MovingObject
{
    public int wallDamage = 1;//daño
    public int pointPerFood = 10;//puntos por comida
    public int pointPerSOda = 20;//puntos por soda
    public float restardLevelDelay = 1f;//segundos antes de reiniciar o pasar al siguente nivel

    private AnimationTree animator;//referencia al nodo animationthree
    private int food;//para llevar la cuenta de cuanta comida tiene
    private AnimationNodeStateMachinePlayback playback;//referencia al nodo animation tree con el parametro para acceder a los stados

    //private bool seMovio = false;

    Wall hitWall;//referencia a la pared que esta chocando

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        movementSpeed = 1f / moveTime;//velocidad a la que se movera
        BoxCollider = GetNode<CollisionShape2D>("CollisionShape2D");
        rayo = GetNode<RayCast2D>("RayCast2D");
        moverConTween = GetNode<Tween>("Tween");
        //rb2D = thi;por ahora voy a evitarla
        animator = GetNode<AnimationTree>("AnimationTree");//referencia al animation three
        _GameManager = (GameManager)GetTree().GetNodesInGroup("GameManager")[0];
        food = _GameManager.playerFoodPoint;//la comida inicial del jugador busco desde el game manager
    
        playback = (AnimationNodeStateMachinePlayback)GetNode<AnimationTree>("AnimationTree").Get("parameters/playback");//accedo al nodo animation three y a la propiedad de las maquinas de estado
        playback.Start("PlayerIdle");//animación inicial player estatico osea igle
        
    }


    private void CheckIfGameOver()//verifica que si es GameOver
    {
        if(food <= 0)//sino tenemos comida
        {
            _GameManager.GameOver();//llamo a la función Game Over,ojo esta función tiene que terminar el juego
        }
    }

    protected override void AttempMove(int xDir, int yDir)
    {
        food --;//en cada paso dismunuje la comida
        base.AttempMove(xDir,yDir);//ejecuto esta acción
        CheckIfGameOver();//siempre que se decrementa la comida verificamos si es game over
        _GameManager.playersTurn = false;//luego terminamos el turno del jugador
    }

    public override void _PhysicsProcess(float delta)
    {
        if(!_GameManager.playersTurn)//si no es el turno del jugador
        {
            return;//dejamos de ejecutar
        }
  
        int horizontal = Convert.ToInt16((Input.GetActionStrength("d") - Input.GetActionStrength("a")) * dimensionSprite);//mover izquierda derecha
        int vertical =  Convert.ToInt16((Input.GetActionStrength("s") - Input.GetActionStrength("w")) * dimensionSprite );//mover arriba abajo
        if(horizontal != 0)//esto es para que NO se mueva en diagonal
        {
            vertical = 0;//esto es para que NO se mueva en diagonal
        }
        if(horizontal != 0 || vertical != 0 )///nos estamos moviendo
        {
            AttempMove(horizontal,vertical);//movemos el personaje
            //if(!seMovio)//esto es para prueba
            //{
            //seMovio = true;
            //
            //}
            //GD.Print(rayo.IsColliding());//esto es para prueba compruebo si el rayo colisiona    
            // await ToSignal(GetTree().CreateTimer(0.1f),"timeout");//esto es para prueba
            //seMovio = false;//esto es para prueba
        }
            
    }
                
            
        


    protected override void OnCantMoveStaticBody2D(StaticBody2D pared)//si "NO" podemos movernos recibe el cuerpo estatico y es un una pared.Este es un metodo que se sobreescribe
    {
        if(pared.IsInGroup("Wall"))
        {
            hitWall = (Wall)pared.GetParent();//con esto tendria que poder acceder al script que maneja ese piso para cambiar la figura al ser golpeado
        }
        if(hitWall != null)//si el personaje esta chocando con un muro
        {
            hitWall.DamageWall(wallDamage);//descuento el hp al suelo,se me esta complicando entender el concepto de unity y pasarlo a godot
            //activo animación romper bloque
            playback.Travel("PlayerChop");//activo animación romper pared
        }
    }
    protected override void OnCantMoveRigidBody2D(KinematicBody2D pared)//si "NO" podemos movernos recibe el cuerpo rigido y es un character.Este es un metodo que se sobreescribe
    {
        playback.Travel("PlayerChop");//activo animación romper pared
    }
        
        
        


    private void Restart()
    {
        GetTree().ReloadCurrentScene();//reinicia la scena esto puede cambiar
    }

    public void LoseFood(int loss)
    {
        food -= loss;//cantidad de comida que pierdo
        playback.Travel("PlayerHit");//activo animación daño
        CheckIfGameOver();//verifico sino todavia tengo comida osea sino es game over
    }

    public async void _on_Area2D_area_entered(Area _area)//esta funcion tiene una corrutina para detener el flujo por un tiempo
    {
        if(_area.IsInGroup("Exit"))//si entramos al aerea de exit
        {
            enable = false;//el jugador no se puede mover
            //aqui tendria que venir una transcición al cambiar de pantalla
            await ToSignal(GetTree().CreateTimer(1.0f),"timeout");//detengo por 1 segundo
            Restart();//reinicio el nivel,voy a tener que utilizar un singleton para guardar puntajes
        }
        if(_area.IsInGroup("Food"))
        {
            food += pointPerFood;//sumo el puntaje de la comida
            (_area.GetParent() as Sprite).Visible = false;//hago invisible el nodo padre que contiene el sprite por lo tanto todos los hijos tendrian que ser inviisble
        }
        if(_area.IsInGroup("Soda"))
        {
            food += pointPerSOda;//sumo el puntaje de la soda
            (_area.GetParent() as Sprite).Visible = false;//hago invisible el nodo padre que contiene el sprite por lo tanto todos los hijos tendrian que ser inviisble
        }
    }

}
