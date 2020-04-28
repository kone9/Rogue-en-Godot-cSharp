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

    private SingletonVariables _SingletonVariables;
    private Label _FoodText;
    //private bool seMovio = false;

    Wall hitWall;//referencia a la pared que esta chocando

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        movementSpeed = 1f / moveTime;//velocidad a la que se movera
        BoxCollider = GetNode<CollisionShape2D>("CollisionShape2D");
        rayo = GetNode<RayCast2D>("RayCast2D");//referencia al raycast
        moverConTween = GetNode<Tween>("Tween");//referencia al nodo tween
        //rb2D = thi;por ahora voy a evitarla
        animator = GetNode<AnimationTree>("AnimationTree");//referencia al animation three
        _GameManager = (GameManager)GetTree().GetNodesInGroup("GameManager")[0];
        
        

        playback = (AnimationNodeStateMachinePlayback)GetNode<AnimationTree>("AnimationTree").Get("parameters/playback");//accedo al nodo animation three y a la propiedad de las maquinas de estado
        playback.Start("PlayerIdle");//animación inicial player estatico osea igle
        
        _SingletonVariables = GetNode<SingletonVariables>("/root/SingletonVariables");//para acceder al singleton desde el player y cambiar el número del nivel y guardar el puntaje
        
        _FoodText = (Label)GetTree().GetNodesInGroup("FoodText")[0];//accedo al nodo label que muestra la comida en la pantalla
        food = _SingletonVariables.food;//la comida inicial del jugador busco desde el game manager
        _FoodText.Text = "food " + food;//comida del jugador
    }


    private void CheckIfGameOver()//verifica que si es GameOver
    {
        if(food <= 0)//sino tenemos comida
        {
            _GameManager.GameOver();//llamo a la función Game Over,ojo esta función tiene que terminar el juego
        }
    }

    protected override Vector2 RaycastDirection(int xDir, int Ydir)//cuando el raycast es del player
    {
        return new Vector2(-Ydir,xDir);//invierto la posición y con esto funciona bien hacia adonde apunta el raycast
    }

    protected override void AttempMove(int xDir, int yDir)
    {
        food --;//en cada paso dismunuje la comida
        _FoodText.Text = "food " + food;//comida del jugador
        base.AttempMove(xDir,yDir);//ejecuto esta acción
        CheckIfGameOver();//siempre que se decrementa la comida verificamos si es game over
        _GameManager.playersTurn = false;//luego terminamos el turno del jugador
    }

    public override void _PhysicsProcess(float delta)
    {
        if(!_GameManager.playersTurn || _GameManager.doingSetup)//si no es el turno del jugador o se esta inciando la escena con el menu
        {
            return;//dejamos de ejecutar y el personaje no se mueve
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
        }
            
    }

    protected override void OnCantMoveStaticBody2D(StaticBody2D pared)//si "NO" podemos movernos recibe el cuerpo estatico y es un una pared.Este es un metodo que se sobreescribe
    {
        if(pared.IsInGroup("Wall"))//si esta en el grupo suelo
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
        _FoodText.Text = " - "+ loss +" food " + food;//comida del jugador que pierde al ser golpeado
        playback.Travel("PlayerHit");//activo animación daño
        CheckIfGameOver();//verifico sino todavia tengo comida osea sino es game over
    }

    public async void _on_Area2D_area_entered(Area _area)//esta funcion tiene una corrutina para detener el flujo por un tiempo
    {
        if(_area.IsInGroup("Exit"))//si entramos al aerea de exit
        {
            enable = false;//el jugador no se puede mover
            //aqui tendria que venir una transcición al cambiar de pantalla
            _SingletonVariables.level += 1;
            _SingletonVariables.food = food;
            await ToSignal(GetTree().CreateTimer(1.0f),"timeout");//detengo por 1 segundo
            Restart();//reinicio el nivel,voy a tener que utilizar un singleton para guardar puntajes
        }
        if(_area.IsInGroup("Food"))
        {
            food += pointPerFood;//sumo el puntaje de la comida
            _FoodText.Text = " + "+ pointPerFood +" food " + food;//comida del jugador
            (_area.GetParent().GetChild(0) as Area2D).Monitoring = false;//para que el cuerpo deje de ser detectado,porque aunque desaparesca el sprite sigue estando la colisión del area detectando cuerpos
            (_area.GetParent() as Sprite).Visible = false;//hago invisible el nodo padre que contiene el sprite por lo tanto todos los hijos tendrian que ser inviisble
        }
        if(_area.IsInGroup("Soda"))
        {
            food += pointPerSOda;//sumo el puntaje de la soda
            _FoodText.Text = " + "+ pointPerSOda +" food " + food;//comida del jugador
            (_area.GetParent().GetChild(0) as Area2D).Monitoring = false;//para que el cuerpo deje de ser detectado,porque aunque desaparesca el sprite sigue estando la colisión del area detectando cuerpos
            (_area.GetParent() as Sprite).Visible = false;//hago invisible el nodo padre que contiene el sprite por lo tanto todos los hijos tendrian que ser inviisble
        }
    }

}
