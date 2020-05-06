using Godot;
using System;
using System.Collections.Generic;//sino esta no funciona la lista

public class BoardManager : Node2D
{
    //esto representa el area donde se puede mover el personaje
    [Export]
    public int columns = 8;//numero de columnas
    [Export]
    public int rows = 8;//numero de filas
    [Export]
    public int FoodQuantityMIN = 1;//cantidad mínima de comida en un escneario
    [Export]
    public int FoodQuantityMAX = 5;//cantidad máxima de comida en un escneario
    [Export]
    public int wallQuantityMIN = 5;//cantidad mínima de obstaculos en un escneario
    [Export]
    public int wallQuantityMAX = 9;//cantidad máxima de obstaculos en un escneario
    //Esto es para contolar la dificultad,puede verse en una grafica como ira aumentando paulantinamente dependiendo cuanto lo multiplique esta variable
    [Export]
    public int controlDificulty = 1;//esta variable se usa para multiplicar la función logaritmica que instancia cantidad de objetos y de esta forma poder aumentar o bajar la cantidad de enemigos,osea manejar la dificultad
    


    [Export]
    private Godot.Collections.Array<PackedScene> floorTiles,outerWallTiles,wallTiles,foodTiles,EnemyTiles;//suelos empaquetados para precargar desde el editor
    [Export]
    private PackedScene exit;
    private List<Vector2> gridPositions = new List<Vector2>();//esta lista guardara todas las posiciones para poder evitar que se superpongan algunos obstaculos y enemigos

    private Sprite toInstantiate;//referencia a los bloques del suelo que seran instanciados

    private Node2D board;//para poner todos los suelos instanciados

    
    public override void _Ready()
    {
        board = (Node2D)GetTree().GetNodesInGroup("Board")[0];//busco el nodo en ese grupo
        GD.Randomize();//creo una semilla aleatoria para que la aleatoriedad siempre sea distinta
    }

    private void InitialiceList()//lista que guarda posiciones
    {
        gridPositions.Clear();//vacia todo el contenido de la lista
        //recorremos con doble bucle para obtener las x y las y como hicimos antes
        //empezamos por el x=1 y=1 para dejar un borde
        for(int x = 1; x < columns - 1; x++)
        {
            for(int y = 1; y < rows -1; y++)
            {
                //creamos un vector 2 con la  X la Y para meterlo en la lista
                gridPositions.Add(new Vector2(x, y));//siempre que uso una posicion tomando en cuenta los sprite multiplico por el tamaño
            }
        }
    }
        
    private Vector2 RandomPosition()
    {
        int randomIndex = (int)GD.RandRange(0,(double)gridPositions.Count);//tengo el total de elementos de la lista
        Vector2 randomPosition = gridPositions[randomIndex];//la posicion aleatorio que hemos obtenido
        
        //una ves que tenemos la posicion la eliminamos de la lista para que no se superpongan
        gridPositions.RemoveAt(randomIndex);//remueve la posicion que acabamos de obtener para que no aparescan encima del otro
        return randomPosition;//nos aseguramos que no duelva una posición que nos dio antes
    }

    private void LayoutObjectAtRandom(Godot.Collections.Array<PackedScene> tileArray,int min,int max)//posicionar objeto en lugar aleatorio//array con todos los muros y creame entre una cantidad minima y maxima de muros
    {
        int objectCount = (int)GD.RandRange(min,(double)max + 1);//numero total de objetos que tenemos que crear
        for(int i = 0; i < objectCount ; i ++)
        {
            Vector2 randomPosition = RandomPosition();//posición aleatoria que recibo de la lista de bloques libres
            Sprite tileChoise = GetRandomInArray(tileArray);//tomamos el nodo que queremos instanciar toma como parametro un arreglo 
            board.AddChild(tileChoise);//hago que sea hijo del nodo board
            //importante multiplicar el random position por el tamaño de sprite osea 32..En el caso del position estariamos haciendo una multiplicaión de 1 número por 1 vector,osea 32*X/32*Y
            tileChoise.Position = randomPosition * 32;//la posicion es la aleatoria que toma en cuenta las posiciones donde pueden estar los obstaculos y evita que se interpongan
            tileChoise.RotationDegrees = 0;//la rotacion es 0      
        }
    }

    //creo una funcion para crear objetos aleatoriamente pero esclusivo para los objetos que son de tipo kinematicos,osea personajes.
    //se ve un poco feo,sin embargo el juego fue diseñado de esta forma
    //sino tendria que cambiar toda la gerarquia de los nodos y no vale la pena
    private void LayoutObjectAtRandomKinematicBody2D(Godot.Collections.Array<PackedScene> tileArray,int min,int max)//posicionar objeto en lugar aleatorio//array con todos los muros y creame entre una cantidad minima y maxima de muros
    {
        int objectCount = (int)GD.RandRange(min,(double)max + 1);//numero total de objetos que tenemos que crear
        for(int i = 0; i < objectCount ; i ++)
        {
            Vector2 randomPosition = RandomPosition();//posición aleatoria que recibo de la lista de bloques libres
            KinematicBody2D tileChoise = GetRandomInArrayKinematicBody(tileArray);//tomamos el nodo que queremos instanciar toma como parametro un arreglo 
            board.AddChild(tileChoise);//hago que sea hijo del nodo board
            //importante multiplicar el random position por el tamaño de sprite osea 32..En el caso del position estariamos haciendo una multiplicaión de 1 número por 1 vector,osea 32*X/32*Y
            tileChoise.Position = randomPosition * 32;//la posicion es la aleatoria que toma en cuenta las posiciones donde pueden estar los obstaculos y evita que se interpongan
            tileChoise.RotationDegrees = 0;//la rotacion es 0      
        }
    }


    public void SetupScene(int level)//esto crea los muros del borde y el suelo..Toma como parametro el nivel para determinar la cantidad de enemigos
    {
        int enemyCount = 0;//inicializo el contador enemigos
        BoardSetup();//prepara la escena básica bordes de alrededor y suelo
        InitialiceList();//metemos en esta lista todas las pocisiones donde pueden aparecer objetos de forma aleatoria
        LayoutObjectAtRandom(wallTiles,wallQuantityMIN,wallQuantityMAX);//cada ves que iniciamos la ejecución nos va a crear entre 5 y 9 muros
        LayoutObjectAtRandom(foodTiles,FoodQuantityMIN,FoodQuantityMAX);//hamos lo mismo pero ahora agregamos la comida habra entre 1 y 5 objetos de comida
        //int enemyCount = level / 2;//cantidad de enemigos en cada nivel.1.2.3.4.etc lo divido por 2 para que la cantidad sea menor y no tenga tanta dificiltad
        if(level == 1)//si es el nivel 1 no hay enemigos
        {
            enemyCount = (int)Math.Log(level,2);//tambien se puede hacer que el incremento de la dificultad sea de forma logaritmica,supuestamente asi esta en el tutorial de unity aqui se usa el logaritmo en "BASE 2" y en este caso es multiplicado por un factor de dificultad
        }
        else if(level == 2)//para que si la dificultad es 2 en el segundo nivel aparesca 1
        {
            enemyCount = (int)Math.Log((level - 1) * controlDificulty,2);//tambien se puede hacer que el incremento de la dificultad sea de forma logaritmica,supuestamente asi esta en el tutorial de unity aqui se usa el logaritmo en "BASE 2" y en este caso es multiplicado por un factor de dificultad
        }
        else//sino la cantidad de enemigos depende de que el nivel sea mayor a 1 y la dificultad
        {
            enemyCount = (int)Math.Log(level * controlDificulty,2);//tambien se puede hacer que el incremento de la dificultad sea de forma logaritmica,supuestamente asi esta en el tutorial de unity aqui se usa el logaritmo en "BASE 2" y en este caso es multiplicado por un factor de dificultad
        }
        LayoutObjectAtRandomKinematicBody2D(EnemyTiles,enemyCount,enemyCount);//Esto crea la cantidad de enemigos dependiendo el nivel,siempre sera 1 número por eso en la aletatoriedad va el mismo número
        instanciarSalida();//ahora instanciamos el nodo que posee la salida,osea el sprite
    }
        

    private void BoardSetup() //metodo para crear el escenario inciial
    {

        //recorremos la X
        for(int x = -1; x < columns + 1; x++ )//si el area es 8x8 quiero tambien incluir en ese area el borde,entonces si la x primera es el cero en -1 iria el muro
        {
            //recorremos la y
            for(int y = -1; y < rows + 1; y++ )//lo mismo pasa en el eje y..Por eso empieza en -1
            {
                toInstantiate = GetRandomInArray(floorTiles);//devuelve un objeto instanciado de tipo sprite toma como parametro un array de packetscene
                
                if(x == -1 || y ==-1 || x == columns || y == rows )//si la posicion pertenece al borde
                {
                    //await ToSignal(GetTree().CreateTimer(1.0f),"timeout");
                    //referencio a un elemento aleatorio del borde
                    toInstantiate = GetRandomInArray(outerWallTiles);//devuelve un objeto isntanciado de tipo sprite toma como parametro un array de packetscene
                }
                
                board.AddChild(toInstantiate);//hago que el sprite instanciado sea hijo del nodo board para poder tener mejor organizados los nodos
                toInstantiate.Position = new Vector2(x * 32 ,y * 32 );//la posición depende de la ubicación en los bucles "for y su X e Y" tambien hay que multiplicar por el tamaño de los sprite
                toInstantiate.RotationDegrees = 0;//la rotación del nodo cero 0 
            }
        }
    }    


    //metodo que devuelve un sprite de suelo aleatoriamente del array que le pasamos por parametro
    private Sprite GetRandomInArray(Godot.Collections.Array<PackedScene> array)
    {
        //referencia al suelo que quiero instanciar uso un rango aleatorio entre los suelos
        //hay que tener cuidado si 1 sprite esta invisible puede parecer que no funciona
        return (Sprite)array[(int)GD.RandRange(0,(double)array.Count)].Instance();//devuelve un sprite aleatorio dependiendo el array que le hemos pasado
    }
        
    //el problema es que el enemigo no es un sprite,creare la mismo función pero para los enemigos y el personaje
    private KinematicBody2D GetRandomInArrayKinematicBody(Godot.Collections.Array<PackedScene> array)
    {
        //referencia al suelo que quiero instanciar uso un rango aleatorio entre los suelos
        //hay que tener cuidado si 1 sprite esta invisible puede parecer que no funciona
        return (KinematicBody2D)array[(int)GD.RandRange(0,(double)array.Count)].Instance();//devuelve un sprite aleatorio dependiendo el array que le hemos pasado
    }

    private void instanciarSalida()
    {
        Sprite tileChoise = (Sprite)exit.Instance();//tomamos el nodo que queremos instanciar toma como parametro un arreglo 
        board.AddChild(tileChoise);//hago que sea hijo del nodo board
        //importante multiplicar el random position por el tamaño de sprite osea 32..En el caso del position estariamos haciendo una multiplicaión de 1 número por 1 vector,osea 32*X/32*Y
        tileChoise.Position = new Vector2(columns - 1,rows - 8) * 32;//la posición del exit esta al borde superior,siemple multiplico por el tamaño del sprite
        tileChoise.RotationDegrees = 0;//la rotacion es 0    
    }

}
