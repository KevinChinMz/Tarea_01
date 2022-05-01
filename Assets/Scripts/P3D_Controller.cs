using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P3D_Controller : MonoBehaviour
{//START CLASS P3D_Controller
    //Variables Publicas
    [Header("Velocidades")]
    public float walkSpeed = 6.75f; //Valor de velocidad al caminar
    public float runSpeed = 10f; //Valor de velocidad al sprintear
    public float crouchSpeed = 4f; //Valor de velocidad al estar agachado

    [Header("Salto")]
    public float jumpSpeed = 8f; //Potencia de salto

    [Header("Valor de Gravedad")]
    public float gravity = 20f; //Valor de gravedad

    public LayerMask groundLayer; //Layer de tierra

    //Variables Privadas
    private float speed; //Velocidad de jugador

    private bool isMoving, isGrounded, isCrouching; //El jugador puede moverse?, Esta en tierra, y si esta agachado?

    private float inputX, inputY; //Flotantes para inputs 
    private float inputXSet, inputYSet; //Preparativos de inputs
    private float inputModifyFactor; //Modificador de los inputs, mov diagonal

    private bool limitDiagonalSpeed = true; //Limitar velocidad diagonal Y/N?

    private float antiBumpFactor = 0.75f; //

    private CharacterController charController; //REF COMP CharacterController del jugador
    private Vector3 moveDirection = Vector3.zero; //Vector de direccion

    //Rayos
    private float rayDistance; //Rayo para calcular distancias --> tierra

    private float defaultControllerHeight; //Valor de la altura normal del controller


    void Start()
    {//START Start
        //Inicializar la ref characterController
        charController = GetComponent<CharacterController>();

        //Declarar la velocidad inicial 
        //Speed privado vale lo mismo que la velocidad de caminado al iniciar el juego
        speed = walkSpeed;

        //El jugador comienza sin movimiento al iniciar el juego
        //Valor inicial de isMoving
        isMoving = false;

        //Inicializacion del rayo de fisicas - tierra
        rayDistance = charController.height * 0.5f + charController.radius;

        //Declaracion de la altura predeterminada del controller
        defaultControllerHeight = charController.height; 
    }//END Start

    void Update()
    {//START Update
        //Llamar al metodo de movimiento del jugador
        PlayerMovement();
    }//END Update

    //Metodo de movimiento
    void PlayerMovement()
    {//START PlayerMovement
        //*************************PREPARAR INPUTS
        //Inputs verticales
        //Vamos a usar W o S para el movimiento vertical
        //Aqui usamos un OR "||": Aqui aplica una o la otra
        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
        {//START IF
            //Escenario W
            //Checar si estamos pulsando la tecla W
            if(Input.GetKey(KeyCode.W))
            {//START IF 2
                //Preparar el valor positivo del input vertical
                inputYSet = 1f;
            }//END IF 2
            //El else es para el caso S
            else
            {//START ELSE 2
                //Preparar el valor negativo del input vertical
                inputYSet = -1f;
            }//END ELSE 2
        }//END IF
        //Este else aplica si no se pica W o S
        else
        {//START ELSE
            //Preparar el valor no existente del input vertical
            inputYSet = 0f;
        }//END ELSE

        //Inputs horizontals
        //Vamos a usar A o D para el movimiento vertical
        if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {//START IF
            //Escenario A
            //Checar si estamos pulsando la tecla A
            if(Input.GetKey(KeyCode.A))
            {//START IF 2
                //Preparar el valor negativo del input horizontal
                inputXSet = -1f;
            }//END IF 2
            //El else es para el caso D
            else
            {//START ELSE 2
                //Preparar el valor positivo del input horizontal
                inputXSet = 1f;
            }//END ELSE 2
        }//END IF
        //Este else aplica si no se pica A o D
        else
        {//START ELSE
            //Preparar el valor no existente del input horizontal
            inputXSet = 0f;
        }//END ELSE

        //**************************APLICACION DE LOS INPUTS
        //Vertical
        inputY = Mathf.Lerp(inputY, inputYSet, Time.deltaTime * 19f);
        
        //Horizontal
        inputX = Mathf.Lerp(inputX, inputXSet, Time.deltaTime * 19f);

        //Dar valor al factor modificador de inputs
        //Arreglar velocidad diagonal
        inputModifyFactor = Mathf.Lerp(inputModifyFactor, 
                                      (inputYSet != 0 && inputXSet != 0 && limitDiagonalSpeed) ? 0.75f : 1.0f,
                                       Time.deltaTime * 19f);

        //Movimiento en tierra
        //Vamos a checar si isGrounded es true
        if(isGrounded)
        {//START IF
            //Llamar a la funcion de agacharse y sprintear
            PlayerCrouchingAndSprinting();

            //movimiento normal del jugador exista si este toca la tierra

            //Valor del vector de direccion
            moveDirection = new Vector3(inputX * inputModifyFactor, 
                                        -antiBumpFactor,
                                        inputY * inputModifyFactor);

            //Valor de direccion 
            //Transformando los valores locales de moveDirection a coordenadas de mundo con la adicion de speed
            moveDirection = transform.TransformDirection(moveDirection) * speed;

            //Llamar a metodo de salto
            PlayerJump();
        }//END IF

        //Aplicacion de gravedad 
        moveDirection.y -= gravity * Time.deltaTime;

        //Determinar el valor de isGrounded
        //Deteccion de tierra si isGrounded es True
        isGrounded = (charController.Move(moveDirection * Time.deltaTime) & CollisionFlags.Below) != 0;

        //Valor del booleano isMoving
        //isMoving sera true si la velocidad del characterController es mayor a un valor minimo
        isMoving = charController.velocity.magnitude > 0.15f;
    }//END PlayerMovement

    //Funcion de features especificos - crouching y sprinting
    void PlayerCrouchingAndSprinting()
    {//START PlayerCrouchingAndSprinting
        //Input para agacharnos
        if(Input.GetKeyDown(KeyCode.C))
        {//START IF
            //Checar si el jugador NO esta agachado
            if(!isCrouching)
            {//START IF 2
                //Vamos a agacharnos, el bool cambia a true
                isCrouching = true;
            }//END IF 2
            else
            //Esto es cuando isCrouching es true
            {//START ELSE 2
                //Levantar al jugador
                //Vamos a checar si nos podemos levantar
                if(CanGetUp())
                {//START IF 2B
                    //Si me puedo levantar entonces isCrouching es falso
                    isCrouching = false;
                }//END IF 2B
            }//END ESLE 2
            //Llamar a la corrutina de agacharse
            StopCoroutine(MoveToCrouchCo());
            StartCoroutine(MoveToCrouchCo());
        }//END IF
       
       //Vamos a checar si isCrouching es true
       //Determinar velocidad al estar agachados
       if(isCrouching)
       {//START IF
            speed = crouchSpeed;
       }//END IF
       //Vamos a aprovechar excepcion para generar sprint
       //SI el jugador NO esta agachado
       else
       {//START ELSE
            //Generar input de sprint
            if(Input.GetKey(KeyCode.LeftShift))
            {//START IF
                //La velocidad va a ser igual a la vel de correr
                speed = runSpeed;
            }//END IF
            else
            //Esto pasa cuando NO pico L Shift
            {//START ELSE
                //La velocidad es igual a la velocidad de caminado
                speed = walkSpeed;
            }//END ELSE
       }//END ELSE
    }//END PlayerCrouchingAndSprinting

    //metodo booleano para saber si nos podemos levantar
    bool CanGetUp()
    {//START CanGetUp
        //Crear un rayo que va ir hacia arriba 
        Ray _groundRay = new Ray(transform.position, transform.up);

        //Crear un raycast hit local
        RaycastHit _groundHit; 

        //Casteo por esfera usando fisicas
        if(Physics.SphereCast(_groundRay, charController.radius,
                              out _groundHit, rayDistance, groundLayer))
        {//START IF
            //Vamos a checar si la distancia entre la posicion actual del jugador y donde pega el rayo es menor a un valor
            if(Vector3.Distance(transform.position, _groundHit.point) < 2.3f)
            {//START IF 2
                //Si esto se cumple regresa false
                return false;
            }//END IF 2
        }//END IF

        //Si nada de esto aplica, regresa true
        return true;
    }//END CanGetUp

    //Corrutina de agacharse, transformarnos para agacharnos
    IEnumerator MoveToCrouchCo()
    {//START MoveToCrouchCo
        //Vamos a determinar la altura del character controller
        charController.height = isCrouching ? charController.height / 2.5f : defaultControllerHeight;

        //Vamos a cambiar el centro del character controller
        charController.center = new Vector3(0f, charController.height / 12f, 0f);

        //Terminar anatomia de corrutina
        yield return null;
    }//END MoveToCrouchCo

    //Funcion de salto
    void PlayerJump()
    {//START PlayerJump
        //Generar el input de salto
        if(Input.GetKeyDown(KeyCode.Space))
        {//START IF
            //Checar si el jugador esta agachado
            if(isCrouching)
            {//START IF 2
                //Vamos a checar si se puede levantar el jugador
                if(CanGetUp())
                {//START IF 3
                    //Si nos podemos levantar vamos  dejar de agacharnos
                    isCrouching = false;

                    //Llamar a la corrutina de agacharse
                    StopCoroutine(MoveToCrouchCo());
                    StartCoroutine(MoveToCrouchCo());
                }//END IF 3
            }//END IF 2
            //Esto es cuando no estoy agachado y quiero saltar
            else
            {//START ELSE 2
                //Cambiar la moveDirection en Y a lo que valga el jumpSpeed
                moveDirection.y = jumpSpeed;
            }//END ELSE 2
        }//END IF
    }//END PlayerJump
}//END CLASS P3D_Controller
