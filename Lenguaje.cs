//Carlos Ramírez Tovar
using System;

//Requerimiento 1.- Actualizar dominante para variables en la expresión V 
//Requerimiento 2.- Actualizar el dominante para el casteo y el valor de la subexpresion V
//Requerimiento 3.- Programar un metodo de conversion de un valor a un tipo de dato V
//                  private float conviert(float valor, String tipoDato);
//                  Deberan de usar el residuo de la division por %255, %65535
//Requerimiento 4.- Evaluar nuevamente la condición del if, else, while, for, doWhile con respecto al parametro que recibe
//Requerimiento 5.- Levantar una excepción en scanf cuando la captura no sea un numero
//Requerimiento 6.- Ejectura el for

namespace Semantica
{
    public class Lenguaje : Sintaxis
    {
        List<Variable> variables = new List<Variable>();
        Stack<float> stack = new Stack<float>();
        Variable.TipoDato dominante;
        public Lenguaje()
        {

        }
        public Lenguaje(string nombre) : base(nombre)
        {

        }
        private void addVariable(string nombre, Variable.TipoDato tipo)
        {
            variables.Add(new Variable(nombre, tipo));
        }
        private void displayVariables()
        {
            foreach (Variable v in variables)
            {
                log.WriteLine(v.getNombre() + " : " + v.getTipoDato() + "\n");
            }
        }
        private float getValor(string nombre)
        {
            foreach (Variable v in variables)
                if (v.getNombre().Equals(nombre))
                    return v.getValor();
            return 0;
        }
        private Variable.TipoDato getTipo(string nombre)
        {
            foreach (Variable v in variables)
            {
                if (v.getNombre().Equals(nombre))
                {
                    return v.getTipoDato();
                }
            }
            return Variable.TipoDato.Char;
        }
        //Programa  -> Librerias? Variables? Main
        public void Programa()
        {
            Libreria();
            Variables();
            Main();
            displayVariables();
        }
        private void modVariable(string nombre, float nuevoValor)
        {
            foreach (var v in variables)
                if (v.getNombre() == nombre)
                    v.setValor(nuevoValor);
        }
        private bool existeVariable(string nombre)
        {
            foreach (Variable v in variables)
            {
                if (v.getNombre().Equals(nombre))
                {
                    return true;
                }
            }
            return false;
        }

        //Librerias -> #include<identificador(.h)?> Librerias?
        private void Libreria()
        {
            if (getContenido() == "#")
            {
                match("#");
                match("include");
                match("<");
                match(Tipos.Identificador);
                if (getContenido() == ".")
                {
                    match(".");
                    match("h");
                }
                match(">");
                Libreria();
            }
        }

        //Variables -> tipo_dato Lista_identificadores; Variables?
        private void Variables()
        {
            if (getClasificacion() == Tipos.TipoDato)
            {
                Variable.TipoDato tipo = Variable.TipoDato.Char;
                switch (getContenido())
                {
                    case "int":
                        tipo = Variable.TipoDato.Int;
                        break;
                    case "float":
                        tipo = Variable.TipoDato.Float;
                        break;
                }
                match(Tipos.TipoDato);
                Lista_identificadores(tipo);
                match(Tipos.FinSentencia);
                Variables();
            }
        }

        //Lista_identificadores -> identificador (,Lista_identificadores)?
        private void Lista_identificadores(Variable.TipoDato tipo)
        {
            if (getClasificacion() == Tipos.Identificador)
            {
                if (!existeVariable(getContenido()))
                {
                    addVariable(getContenido(), tipo);
                }
                else
                {
                    throw new Error("Error de sintaxis, variable duplicada <" + getContenido() + "> en linea: " + linea, log);
                }
            }
            match(Tipos.Identificador);
            if (getContenido() == ",")
            {
                match(",");
                Lista_identificadores(tipo);
            }
        }
        //Main      -> void main() Bloque de instrucciones
        private void Main()
        {
            match("void");
            match("main");
            match("(");
            match(")");
            BloqueInstrucciones(true);
        }

        //Bloque de instrucciones -> {listaIntrucciones?}
        private void BloqueInstrucciones(bool evaluacion)
        {
            match("{");
            if (getContenido() != "}")
            {
                ListaInstrucciones(evaluacion);
            }
            match("}");
        }

        //ListaInstrucciones -> Instruccion ListaInstrucciones?
        private void ListaInstrucciones(bool evaluacion)
        {
            Instruccion(evaluacion);
            if (getContenido() != "}")
            {
                ListaInstrucciones(evaluacion);
            }
        }

        //ListaInstruccionesCase -> Instruccion ListaInstruccionesCase?
        private void ListaInstruccionesCase(bool evaluacion)
        {
            Instruccion(evaluacion);
            if (getContenido() != "case" && getContenido() != "break" && getContenido() != "default" && getContenido() != "}")
            {
                ListaInstruccionesCase(evaluacion);
            }
        }

        //Instruccion -> Printf | Scanf | If | While | do while | For | Switch | Asignacion
        private void Instruccion(bool evaluacion)
        {
            if (getContenido() == "printf")
            {
                Printf(evaluacion);
            }
            else if (getContenido() == "scanf")
            {
                Scanf(evaluacion);
            }
            else if (getContenido() == "if")
            {
                If(evaluacion);
            }
            else if (getContenido() == "while")
            {
                While(evaluacion);
            }
            else if (getContenido() == "do")
            {
                Do(evaluacion);
            }
            else if (getContenido() == "for")
            {
                For(evaluacion);
            }
            else if (getContenido() == "switch")
            {
                Switch(evaluacion);
            }
            else
            {
                Asignacion(evaluacion);
            }
        }
        private Variable.TipoDato evaluaNumero(float resultado)
        {
            if (resultado % 1 != 0)
            {
                return Variable.TipoDato.Float;
            }
            if (resultado <= 255)
            {
                return Variable.TipoDato.Char;
            }
            else if (resultado <= 65535)
            {
                return Variable.TipoDato.Int;
            }
            else
            {
                return Variable.TipoDato.Float;
            }
        }
        private bool evaluaSemantica(string variable, float resultado)
        {
            Variable.TipoDato tipoDato = getTipo(variable);
            return false;
        }

        private float convierte(float valor, Variable.TipoDato tipoDato)
        {
            if (tipoDato == Variable.TipoDato.Char)
            {
                return valor % 256;
            }
            if (tipoDato == Variable.TipoDato.Int)
            {
                return valor % 65536;
            }
            return valor;
        }

        //Asignacion -> identificador = cadena | Expresion;
        private void Asignacion(bool evaluacion)
        {
            string nombre = getContenido();
            if (!existeVariable(getContenido()))
                throw new Error("Error: No existe la variable " + getContenido() + " en linea: " + linea, log);
            match(Tipos.Identificador);
            //log.WriteLine();
           // log.Write(getContenido() + " = ");
            match(Tipos.Asignacion);
            dominante = Variable.TipoDato.Char;
            Expresion();
            match(";");
            float resultado = stack.Pop();
            Console.WriteLine(dominante);
            Console.WriteLine(evaluaNumero(resultado));
            //log.Write(" = " + resultado);
            //
            if (dominante < evaluaNumero(resultado))
            {
                Console.WriteLine("flag: " + evaluaNumero(resultado));
                dominante = evaluaNumero(resultado);
            }
            //
            if (dominante <= getTipo(nombre))
            {
                if (evaluacion)
                {
                    modVariable(nombre, resultado);
                }
            }
            else
            {
                throw new Error("Error de semantica no podemos asignar un <" + dominante + "> a un <" + getTipo(nombre) + "> en linea: " + linea, log);
            }
        }

        //While -> while(Condicion) bloque de instrucciones | instruccion
        private void While(bool evaluacion)
        {
            bool validarWhile;
            match("while");
            match("(");
            if(evaluacion){
                validarWhile = Condicion();//Requerimiento 4
            }else {
                Condicion();
                validarWhile = false;
            }
            match(")");
            if (getContenido() == "{")
            {
                BloqueInstrucciones(validarWhile);
            }
            else
            {
                Instruccion(validarWhile);
            }
        }

        //Do -> do bloque de instrucciones | intruccion while(Condicion)
        private void Do(bool evaluacion)
        {
            bool validarDo = evaluacion;
            match("do");
            if (getContenido() == "{")
            {
                BloqueInstrucciones(validarDo);
            }
            else
            {
                Instruccion(validarDo);
            }
            match("while");
            match("(");
            if(evaluacion){
                validarDo = Condicion();//Requerimiento 4
            }else {
                Condicion();
                validarDo = false;
            } //Requerimiento 4
            match(")");
            match(";");
        }
        //For -> for(Asignacion Condicion; Incremento) BloqueInstruccones | Intruccion 
        private void For(bool evaluacion)
        {
            bool validarFor;
            match("for");
            match("(");
            Asignacion(evaluacion);
            int pos = getPosicion();
            //Requerimiento 4
            //Requerimiento 6: 
            //a) Guardar la posición del archivo de texto
            //b) Agregar un ciclo while y validar la condición   
            //do{ 
                Console.WriteLine(contenidoC());
                SetPosition(pos + 5);
                Console.WriteLine(contenidoC());
                if(evaluacion){
                    validarFor = Condicion();//Requerimiento 4
                }
                else 
                {
                    Condicion();
                    validarFor = false;
                }
                match(";");
                Incremento(validarFor);
                match(")");
                if (getContenido() == "{")
                {
                    BloqueInstrucciones(validarFor);
                }
                else
                {
                    Instruccion(validarFor);
                }
            //}while(validarFor);
            //d) Sacar otro token
        }

        //Incremento -> Identificador ++ | --
        private void Incremento(bool evaluacion)
        {
            string Variable = getContenido();
            if (!existeVariable(getContenido()))
                throw new Error("Error: No existe la variable " + getContenido() + " en linea: " + linea, log);
            match(Tipos.Identificador);
            if (getContenido() == "++")
            {
                if (evaluacion)
                {
                    modVariable(Variable, getValor(Variable) + 1);
                }
                match("++");
            }
            else
            {
                if (evaluacion)
                {
                    modVariable(Variable, getValor(Variable) - 1);
                }
                match("--");
            }
        }

        //Switch -> switch (Expresion) {Lista de casos} | (default: )
        private void Switch(bool evaluacion)
        {
            match("switch");
            match("(");
            Expresion();
            stack.Pop();
            match(")");
            match("{");
            ListaDeCasos(evaluacion);
            if (getContenido() == "default")
            {
                match("default");
                match(":");
                if (getContenido() == "{")
                {
                    BloqueInstrucciones(evaluacion);
                }
                else
                {
                    Instruccion(evaluacion);
                }
            }
            match("}");
        }

        //ListaDeCasos -> case Expresion: listaInstruccionesCase (break;)? (ListaDeCasos)?
        private void ListaDeCasos(bool evaluacion)
        {
            match("case");
            Expresion();
            stack.Pop();
            match(":");
            ListaInstruccionesCase(evaluacion);
            if (getContenido() == "break")
            {
                match("break");
                match(";");
            }
            if (getContenido() == "case")
            {
                ListaDeCasos(evaluacion);
            }
        }

        //Condicion -> Expresion operador relacional Expresion
        private bool Condicion()
        {
            float e1, e2;
            Expresion();
            string operador = getContenido();
            match(Tipos.OperadorRelacional);
            Expresion();
            e2 = stack.Pop();
            e1 = stack.Pop();
            switch (operador)
            {
                case "==":
                    return e1 == e2;
                case ">":
                    return e1 > e2;
                case ">=":
                    return e1 >= e2;
                case "<":
                    return e1 < e2;
                case "<=":
                    return e1 <= e2;
                default:
                    return e1 != e2;
            }
        }

        //If -> if(Condicion) bloque de instrucciones (else bloque de instrucciones)?
        private void If(bool evaluacion)
        {
            bool validarIf;
            match("if");
            match("(");
            if(evaluacion){
                validarIf = Condicion();//Requerimiento 4
            }else {
                Condicion();
                validarIf = false;
            }
            match(")");
            if (getContenido() == "{")
            {
                BloqueInstrucciones(validarIf);
            }
            else
            {
                Instruccion(validarIf);
            }
            if (getContenido() == "else")//Requerimiento 4
            {
                match("else");
                if (getContenido() == "{")
                {
                    BloqueInstrucciones(!validarIf);
                }
                else
                {
                    Instruccion(!validarIf);
                }
            }
        }

        //Printf -> printf(cadena|expresion);
        private void Printf(bool evaluacion)
        {
            match("printf");
            match("(");
            if (getClasificacion() == Tipos.Cadena)
            {
                setContenido(getContenido().Replace("\\t", "    "));
                setContenido(getContenido().Replace("\\n", "\n"));
                setContenido(getContenido().Replace("\"", string.Empty));
                if (evaluacion)
                {
                    Console.Write(getContenido());
                }
                match(Tipos.Cadena);
            }
            else
            {
                if (evaluacion)
                {
                    Expresion();
                    Console.WriteLine(stack.Pop());
                }
            }
            match(")");
            match(";");
        }

        //Scanf -> scanf(cadena,&identificador);
        private void Scanf(bool evaluacion)
        {
            match("scanf");
            match("(");
            match(Tipos.Cadena);
            match(",");
            match("&");
            if (!existeVariable(getContenido()))
                throw new Error("Error: No existe la variable " + getContenido() + " en linea: " + linea, log);
            if(evaluacion)
            {
                string val = "" + Console.ReadLine(); 
                if(val.All(char.IsDigit))
                {
                    modVariable(getContenido(), float.Parse(val));
                }
                else
                {
                    throw new Error("Error: No se puede asignar un valor no numerico a una variable numerica en linea: " + linea, log);
                }
            }
            match(Tipos.Identificador);
            match(")");
            match(";");
        }

        

        //Expresion -> Termino MasTermino
        private void Expresion()
        {
            Termino();
            MasTermino();
        }
        //MasTermino -> (OperadorTermino Termino)?
        private void MasTermino()
        {
            if (getClasificacion() == Tipos.OperadorTermino)
            {
                string operador = getContenido();
                match(Tipos.OperadorTermino);
                Termino();
                //log.Write(operador + " ");
                float n1 = stack.Pop();
                float n2 = stack.Pop();
                switch (operador)
                {
                    case "+":
                        stack.Push(n2 + n1);
                        break;
                    case "-":
                        stack.Push(n2 - n1);
                        break;
                }
            }
        }
        //Termino -> Factor PorFactor
        private void Termino()
        {
            Factor();
            PorFactor();
        }
        //PorFactor -> (OperadorFactor Factor)? 
        private void PorFactor()
        {
            if (getClasificacion() == Tipos.OperadorFactor)
            {
                string operador = getContenido();
                match(Tipos.OperadorFactor);
                Factor();
                //log.Write(operador + " ");
                float n1 = stack.Pop();
                float n2 = stack.Pop();
                switch (operador)
                {
                    case "*":
                        stack.Push(n2 * n1);
                        break;
                    case "/":
                        stack.Push(n2 / n1);
                        break;
                }
            }
        }
        //Factor -> numero | identificador | (Expresion)
        private void Factor()
        {
            if (getClasificacion() == Tipos.Numero)
            {
                //log.Write(getContenido() + " ");
                if (dominante < evaluaNumero(float.Parse(getContenido())))
                {
                    dominante = evaluaNumero(float.Parse(getContenido()));
                }
                stack.Push(float.Parse(getContenido()));
                match(Tipos.Numero);
            }
            else if (getClasificacion() == Tipos.Identificador)
            {
                stack.Push(getValor(getContenido()));
                if (!existeVariable(getContenido()))
                {
                    throw new Error("Error: No existe la variable " + getContenido() + " en linea: " + linea, log);
                }
                if (dominante < getTipo(getContenido())) //Requisito 1
                {
                    dominante = getTipo(getContenido());
                }
                match(Tipos.Identificador);
            }
            else
            {
                bool huboCasteo = false;
                Variable.TipoDato casteo = Variable.TipoDato.Char;
                match("(");
                if (getClasificacion() == Tipos.TipoDato)
                {
                    huboCasteo = true;
                    switch (getContenido())
                    {
                        case "char":
                            casteo = Variable.TipoDato.Char;
                            break;
                        case "int":
                            casteo = Variable.TipoDato.Int;
                            break;
                        case "float":
                            casteo = Variable.TipoDato.Float;
                            break;
                    }
                    match(Tipos.TipoDato);
                    match(")");
                    match("(");
                }
                Expresion();
                match(")");
                if (huboCasteo)
                {
                    dominante = casteo;
                    float valor = stack.Pop();
                    if (valor%1 != 0 && casteo != Variable.TipoDato.Float)
                    {
                        valor = (float) Math.Truncate(valor);
                    }
                    stack.Push(convierte(valor, casteo));
                    //Requerimiento 2 - Sacar un elmento del stack
                    //                  Convierto ese valor al equivalente en casteo
                    //Requeremiento 3
                    //                  Si el casteo es char y el pop regresa un 256, 
                    //                  el valor equivalente en casteo es 0
                }
            }
        }
    }
}