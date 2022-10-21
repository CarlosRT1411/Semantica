//Carlos Ramírez Tovar
using System;

//Requerimiento 1.- Actualización: a)Agregar el residuo de la división en por factor
//                                 b)Agregar en instrucción los incrementos de termino y los incrementos de factor
//                                 c)PRogramar el destructor para ejecutar el metodo cerrar archivo
//Requerimiento 2.-
//                                 a)Marcar errores semanticos cuando los incrementos de termino o incrementos de factor superen el rango de la variable
//                                 b)Considerar inciso b y c para el for
//                                 c)Que funcione el do y el while
//Requerimiento 3.-                a)Considerar las variables y los casteos de las expresiones matematicas en ensamblador
//                                 b)Considerar el residuo de la división en ensamblador
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
        ~Lenguaje(){
            Console.WriteLine("Destructor");
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
        private void variablesAsm()
        {
            foreach (Variable v in variables)
            {
                asm.WriteLine("\t" + v.getNombre() + " dw ?");
                log.WriteLine(v.getNombre() + " : " + v.getTipoDato() + "\n");
            }
        }
        private float getValor(string nombre)
        {
            foreach (Variable v in variables)
                if (v.getNombre().Equals(nombre))
                {
                    return v.getValor();
                }
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
        public void SetPosicion(long posicion)
        {
            archivo.DiscardBufferedData();
            archivo.BaseStream.Seek(posicion, SeekOrigin.Begin);
        }
        public void Programa()
        {
            asm.WriteLine("#make_COM#");
            asm.WriteLine("include emu8086.inc");
            asm.WriteLine("ORG 100h");
            Libreria();
            Variables();
            variablesAsm();
            Main();
            displayVariables();
            asm.WriteLine("ret");
            asm.WriteLine("END");
        }
        private void modVariable(string nombre, float nuevoValor)
        {
            foreach (var v in variables)
            {
                if (v.getNombre() == nombre)
                {
                    v.setValor(nuevoValor);
                }
            }
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
                return (char) valor % 256;
            }
            if (tipoDato == Variable.TipoDato.Int)
            {
                return (int) valor % 65536;
            }
            return valor;
        }

        //Asignacion -> identificador = cadena | Expresion;
        private void Asignacion(bool evaluacion)
        {
            string nombre = getContenido();
            if (!existeVariable(getContenido()))
            {
                throw new Error("Error: No existe la variable " + getContenido() + " en linea: " + linea, log);
            }
            match(Tipos.Identificador);
            //Si la getClasificacion es == a un incrementoTermino  o incrementoFactor
                //Requerimiento 1.b
                //Requerimiento 1.c
            //else
            match(Tipos.Asignacion);
            dominante = Variable.TipoDato.Char;
            Expresion();
            match(";");
            float resultado = stack.Pop();
            asm.WriteLine("POP AX");
            if (dominante < evaluaNumero(resultado))
            {
                dominante = evaluaNumero(resultado);
            }
            if (dominante <= getTipo(nombre))
            {
                if (evaluacion)
                {
                    asm.WriteLine("MOV " + nombre + ", AX");
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
            match("while");
            match("(");
            bool validarWhile = Condicion();
            if (!evaluacion)
            {
                validarWhile = evaluacion;
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
            validarDo = Condicion();
            if (!evaluacion)
            {
                validarDo = evaluacion;
            }
            match(")");
            match(";");
        }
        //For -> for(Asignacion Condicion; Incremento) BloqueInstruccones | Intruccion 
private void For(bool evaluacion)
        {
            match("for");
            match("(");
            Asignacion(evaluacion);
            string variable = getContenido();
            bool validarFor;
            int posFor = posicion;
            int linFor = linea;
            do
            {
                validarFor = Condicion();
                if(!evaluacion)
                {
                    validarFor = false;
                }
                match(";");
                Incremento(validarFor);
                match(")");
                if(getContenido() == "{")
                {
                    BloqueInstrucciones(validarFor);  
                }
                else
                {
                    Instruccion(validarFor);
                }
                if(validarFor)
                {
                    posicion = posFor - variable.Length;
                    linea = linFor;
                    SetPosicion(posicion);
                    NextToken();
                }
            }while(validarFor);
        }

        //Incremento -> Identificador ++ | --
        private void Incremento(bool evaluacion)
        {
            string Variable = getContenido();
            if (!existeVariable(getContenido()))
            {
                throw new Error("Error: No existe la variable " + getContenido() + " en linea: " + linea, log);
            }
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
            asm.WriteLine("POP AX");
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
            asm.WriteLine("POP AX");
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
            asm.WriteLine("POP AX");
            e1 = stack.Pop();
            asm.WriteLine("POP BX");
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
            match("if");
            match("(");
            bool validarIf = Condicion();
            if (!evaluacion)
            {
                validarIf = evaluacion;
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
                    if (evaluacion)
                    {
                        BloqueInstrucciones(!validarIf);
                    }
                    else
                    {
                        BloqueInstrucciones(evaluacion);
                    }
                }
                else
                {
                    if (evaluacion)
                    {
                        Instruccion(!validarIf);
                    }
                    else
                    {
                        Instruccion(evaluacion);
                    }
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
                Expresion();
                if (evaluacion)
                {
                    Console.Write(stack.Pop());     
                    asm.WriteLine("POP AX");             
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
            if (evaluacion)
            {
                string val = "" + Console.ReadLine();
                float n;
                if (float.TryParse(val, out n))
                {
                    modVariable(getContenido(), n);
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
                asm.WriteLine("POP BX");
                float n2 = stack.Pop();
                asm.WriteLine("POP AX");
                switch (operador)
                {
                    case "+":
                        stack.Push(n2 + n1);
                        asm.WriteLine("ADD AX, BX");  
                        asm.WriteLine("PUSH AX");
                        break;
                    case "-":
                        stack.Push(n2 - n1);
                        asm.WriteLine("SUB AX, BX");
                        asm.WriteLine("PUSH AX");
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
                float n1 = stack.Pop();
                asm.WriteLine("POP BX");
                float n2 = stack.Pop();
                asm.WriteLine("POP AX");
                //Requerimiento 1.a
                switch (operador)
                {
                    case "*":
                        stack.Push(n2 * n1);
                        asm.WriteLine("MUL BX");
                        asm.WriteLine("PUSH AX");
                        break;
                    case "/":
                        stack.Push(n2 / n1);
                        asm.WriteLine("DIV BX ");
                        asm.WriteLine("PUSH AX");
                        break;
                    case "%": //Esto no se si funciona
                        stack.Push(n2 % n1);
                        asm.WriteLine("DIV BX ");
                        asm.WriteLine("PUSH DX");
                        break;
                }
            }
        }
        //Factor -> numero | identificador | (Expresion)
        private void Factor()
        {
            if (getClasificacion() == Tipos.Numero)
            {
                if (dominante < evaluaNumero(float.Parse(getContenido())))
                {
                    dominante = evaluaNumero(float.Parse(getContenido()));
                }
                stack.Push(float.Parse(getContenido()));
                asm.WriteLine("MOV AX, " + getContenido());
                asm.WriteLine("PUSH AX");
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
                    asm.WriteLine("POP AX");
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