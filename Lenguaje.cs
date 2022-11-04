//Carlos Ramírez Tovar
using System;

//Requerimiento 1.- Actualización: a)Agregar el residuo de la división en por factor V
//                                 b)Agregar en instrucción los incrementos de termino y los incrementos de factor V
//                                 c)Programar el destructor para ejecutar el metodo cerrar archivo V

//Requerimiento 2.-                a)Marcar errores semanticos cuando los incrementos de termino o incrementos de factor superen el rango de la variable V
//                                 b)Considerar inciso a y b para el for V
//                                 c)Que funcione el do y el while V 

//Requerimiento 3.-                a)Considerar las variables y los casteos de las expresiones matematicas en ensamblador V
//                                 b)Considerar el residuo de la división en ensamblador V
//                                 c)Programar el printf y scanf V

//Requerimiento 4.-                a)Programar el else en ensamblador V
//                                 b)Progrmar el for en ensamblador V

//Requerimiento 5.-                a)Programar el while en ensamblador
//                                 b)Programar el do while en ensamblador V
namespace Semantica
{
    public class Lenguaje : Sintaxis, IDisposable
    {
       
        List<Variable> variables = new List<Variable>();
        Stack<float> stack = new Stack<float>();
        Variable.TipoDato dominante;
        int cIf, cFor, cDo, cWhile;
        string asmIncrementar;
        public Lenguaje()
        {
            cIf = cFor = cDo = cWhile= 0;
            asmIncrementar = "";
        }
        public Lenguaje(string nombre) : base(nombre)
        {
            cIf = cFor = cDo = cWhile= 0;
            asmIncrementar = "";
        }

        public void Dispose()
        {
            cerrar(); 
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
            asm.WriteLine("\n;Variables");
            foreach (Variable v in variables)
            {
                if(v.getTipoDato() == Variable.TipoDato.Char)
                {
                    asm.WriteLine("\t" + v.getNombre() + " db ?"); //db = define byte
                }
                else if(v.getTipoDato() == Variable.TipoDato.Int)
                {
                    asm.WriteLine("\t" + v.getNombre() + " dw ?"); //dw = define word
                }
                else
                {
                    asm.WriteLine("\t" + v.getNombre() + " dd ?"); //dd = define double
                }
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
        public void saltosASM(string cadena)
        {
            string [] sub = cadena.Split("\\n");
            int i = 0;
            foreach (string s in sub) 
            {  
                if(i == sub.Length - 1)
                {
                    asm.WriteLine("PRINT \'" + s + "\'");
                }
                else
                {
                    asm.WriteLine("PRINTN \'" + s + "\'");
                }
                i++;
            }
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
            Main();
            displayVariables();
            variablesAsm();
            asm.WriteLine("ret");
            asm.WriteLine("define_print_num");
            asm.WriteLine("define_print_num_uns");
            asm.WriteLine("define_scan_num");
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
            BloqueInstrucciones(true, true);
        }

        //Bloque de instrucciones -> {listaIntrucciones?}
        private void BloqueInstrucciones(bool evaluacion, bool imprimir)
        {
            match("{");
            if (getContenido() != "}")
            {
                ListaInstrucciones(evaluacion, imprimir);
            }
            match("}");
        }

        //ListaInstrucciones -> Instruccion ListaInstrucciones?
        private void ListaInstrucciones(bool evaluacion, bool imprimir)
        {
            Instruccion(evaluacion, imprimir);
            if (getContenido() != "}")
            {
                ListaInstrucciones(evaluacion, imprimir);
            }
        }

        //ListaInstruccionesCase -> Instruccion ListaInstruccionesCase?
        private void ListaInstruccionesCase(bool evaluacion, bool imprimir)
        {
            Instruccion(evaluacion, imprimir);
            if (getContenido() != "case" && getContenido() != "break" && getContenido() != "default" && getContenido() != "}")
            {
                ListaInstruccionesCase(evaluacion, imprimir);
            }
        }

        //Instruccion -> Printf | Scanf | If | While | do while | For | Switch | Asignacion
        private void Instruccion(bool evaluacion, bool imprimir)
        {
            switch (getContenido())
            {
                case "printf":
                    Printf(evaluacion, imprimir);
                    break;
                case "scanf":
                    Scanf(evaluacion, imprimir);
                    break;
                case "if":
                    If(evaluacion, imprimir);
                    break;
                case "while":
                    While(evaluacion, imprimir);
                    break;
                case "do":
                    Do(evaluacion, imprimir);
                    break;
                case "for":
                    For(evaluacion, imprimir);
                    break;
                case "switch":
                    Switch(evaluacion, imprimir);
                    break;
                default:
                    Asignacion(evaluacion, imprimir);
                    break;
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
                return (char)valor % 256;
            }
            if (tipoDato == Variable.TipoDato.Int)
            {
                return (int)valor % 65536;
            }
            return valor;
        }

        //Asignacion -> identificador = cadena | Expresion;
        private void Asignacion(bool evaluacion, bool imprimir)
        {
            string nombre = getContenido();
            if (!existeVariable(getContenido()))
            {
                throw new Error("Error: No existe la variable " + getContenido() + " en linea: " + linea, log);
            }
            match(Tipos.Identificador);
            if (getClasificacion() == Tipos.IncrementoTermino || getClasificacion() == Tipos.IncrementoFactor)
            {
                modVariable(nombre, Incremento(evaluacion, nombre, imprimir));
                match(";");
                if(imprimir){
                    asm.WriteLine(asmIncrementar);
                }
            }
            else
            {
                match(Tipos.Asignacion);
                dominante = Variable.TipoDato.Char;
                Expresion(imprimir);
                match(";");
                float resultado = stack.Pop();
                if(imprimir)
                {
                    asm.WriteLine("POP AX");
                }
                if (dominante < evaluaNumero(resultado))
                {
                    dominante = evaluaNumero(resultado);
                }
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
                if(getTipo(nombre) == Variable.TipoDato.Char)
                {
                    if(imprimir)
                    {
                        asm.WriteLine("MOV " + nombre + ", AL");
                    }
                }
                else 
                {
                    if(imprimir)
                    {
                        asm.WriteLine("MOV " + nombre + ", AX");
                    }
                }
            }
        }

        //While -> while(Condicion) bloque de instrucciones | instruccion
        private void While(bool evaluacion, bool imprimir)
        {
            if(imprimir)
            {
                cWhile++;
            }
            string etiquetaWhileInicio = "whileInicio" + cWhile + ":";
            string etiquetaWhileFin = "whileFin" + cWhile + ":";
            match("while");
            match("(");
            bool validarWhile;
            String variable = getContenido();
            int posFor = posicion;
            int linFor = linea;
            do
            {
                if(imprimir)
                {
                    asm.WriteLine(etiquetaWhileInicio);
                }
                validarWhile = Condicion(etiquetaWhileFin, imprimir);
                if (!evaluacion)
                {
                    validarWhile = evaluacion;
                }
                match(")");
                if (getContenido() == "{")
                {
                    BloqueInstrucciones(validarWhile, imprimir);
                }
                else
                {
                    Instruccion(validarWhile, imprimir);
                }
                if (validarWhile)
                {
                    posicion = posFor - variable.Length;
                    linea = linFor;
                    SetPosicion(posicion);
                    NextToken();
                }
                if(imprimir)
                {
                    asm.WriteLine("JMP " + etiquetaWhileInicio);
                    asm.WriteLine(etiquetaWhileFin);
                }
                imprimir = false; 
            } while (validarWhile);
        }

        //Do -> do bloque de instrucciones | intruccion while(Condicion)
        private void Do(bool evaluacion, bool imprimir)
        {
            bool validarDo = evaluacion;
            if(imprimir) 
            {
                ++cDo; 
            }
            string etiquetaInicioDo = "inicioDo" + cDo;
            string etiquetaFinDo = "finDo" + cDo;
            string variable;
            match("do");
            int posFor = posicion;
            int linFor = linea;
            do
            {
                if(imprimir)
                {
                    asm.WriteLine(etiquetaInicioDo + ":");
                }
                if (getContenido() == "{")
                {
                    BloqueInstrucciones(validarDo, imprimir);
                }
                else
                {
                    Instruccion(validarDo, imprimir);
                }
                match("while");
                match("(");
                variable = getContenido();
                validarDo = Condicion(etiquetaFinDo, imprimir);
                if (!evaluacion)
                {
                    validarDo = evaluacion;
                }
                else if(validarDo) 
                {
                    posicion = posFor - 1;
                    linea = linFor;
                    SetPosicion(posicion);
                    NextToken();
                }
                if(imprimir)
                {
                    asm.WriteLine("JMP " + etiquetaInicioDo);
                    asm.WriteLine(etiquetaFinDo + ":");
                }
                imprimir = false;
            }while (validarDo);
            match(")");
            match(";");
        }
        //For -> for(Asignacion Condicion; Incremento) BloqueInstruccones | Intruccion 
        private void For(bool evaluacion, bool imprimir)
        {
            bool validarFor;
            if(imprimir) 
            {
                ++cFor; 
            }
            string etiquetaInicioFor = "InicioFor" + cFor;
            string etiquetaFinFor = "finFor" + cFor;
            string asmMod = "";
            match("for");
            match("(");
            Asignacion(evaluacion, imprimir);
            string variable = getContenido();
            float incrementar = 0, valorInicial = getValor(variable);
            int posFor = posicion, linFor = linea;
            do
            {
                if(imprimir)
                {
                    asm.WriteLine(etiquetaInicioFor + ":");
                }
                validarFor = Condicion(etiquetaFinFor, imprimir);
                if (!evaluacion)
                {
                    validarFor = false;
                }
                match(";");
                match(Tipos.Identificador);
                incrementar = Incremento(validarFor, variable, imprimir);
                asmMod = asmIncrementar;
                match(")");
                if (getContenido() == "{")
                {
                    BloqueInstrucciones(validarFor, imprimir);
                }
                else
                {
                    Instruccion(validarFor, imprimir);
                }
                if (validarFor)
                {
                    posicion = posFor - variable.Length;
                    linea = linFor;
                    if(valorInicial == getValor(variable))
                    {
                        modVariable(variable, incrementar);
                    }
                    valorInicial = getValor(variable);
                    SetPosicion(posicion);
                    NextToken();
                }
                if(imprimir)
                {
                    asm.WriteLine(asmMod);
                    asm.WriteLine("JMP " + etiquetaInicioFor);
                    asm.WriteLine(etiquetaFinFor + ":");
                }
                imprimir = false;
            } while (validarFor);
        }

        //Incremento -> Identificador ++ | --
        private float Incremento(bool evaluacion, string Variable, bool imprimir)
        {
            float variableModificada = getValor(Variable), resultado = 0;
            if (!existeVariable(Variable))
            {
                throw new Error("Error: No existe la variable " + getContenido() + " en linea: " + linea, log);
            }
            if (getContenido() == "++")
            {
                if(imprimir)
                {
                    asmIncrementar = "INC " + Variable;
                }
                if (evaluacion)
                {
                    variableModificada++;
                }
                if(getTipo(Variable) == Semantica.Variable.TipoDato.Char && variableModificada > 255)
                {
                    throw new Error("Error: El valor de la variable " + Variable + " excede el rango de un char en linea: " + linea, log);
                }
                else if(getTipo(Variable) == Semantica.Variable.TipoDato.Int && variableModificada > 65535)
                {
                    throw new Error("Error: El valor de la variable " + Variable + " excede el rango permitido en linea: " + linea, log);
                }
                match("++");
            }
            else if (getContenido() == "--")
            {
                if(imprimir)
                {
                    asmIncrementar = "DEC " + Variable;
                }
                if (evaluacion)
                {
                    variableModificada--;
                }
                match("--");
            }
            else if (getContenido() == "+=")
            {
                match("+=");
                Expresion(imprimir);
                resultado = stack.Pop();
                if (imprimir)
                {
                    asmIncrementar = "POP AX";
                    if(getTipo(Variable) > Semantica.Variable.TipoDato.Char) 
                    {
                        asmIncrementar += "\nMOV BX, " + Variable;
                        asmIncrementar += "\nADD BX, AX ";
                        asmIncrementar += "\nMOV " + Variable + ", BX";
                    }
                    else 
                    {
                        asmIncrementar += "\nMOV BL, " + Variable;
                        asmIncrementar += "\nADD BL, AL ";
                        asmIncrementar += "\nMOV " + Variable + ", BL";
                    }
                }
                if (evaluacion)
                {
                    variableModificada += resultado;
                }
            }
            else if (getContenido() == "-=")
            {
                match("-=");
                Expresion(imprimir);
                resultado = stack.Pop();
                if (imprimir)
                {   
                    asmIncrementar = "POP AX";
                    if(getTipo(Variable) > Semantica.Variable.TipoDato.Char)
                    {
                        asmIncrementar += "\nMOV BX, " + Variable;
                        asmIncrementar += "\nSUB BX, AX ";
                        asmIncrementar += "\nMOV " + Variable + ", BX";
                    }
                    else
                    {
                        asmIncrementar += "\nMOV BL, " + Variable;
                        asmIncrementar += "\nSUB BL, AL ";
                        asmIncrementar += "\nMOV " + Variable + ", BL";                        
                    }
                }
                if (evaluacion)
                {
                    variableModificada -= resultado;
                }
            }
            else if (getContenido() == "*=")
            {
                match("*=");
                Expresion(imprimir);
                resultado = stack.Pop();
                if (imprimir)
                {
                    asmIncrementar = "POP AX";
                    if(getTipo(Variable) > Semantica.Variable.TipoDato.Char)
                    {    
                        asmIncrementar += "\nMOV BX, " + Variable;
                        asmIncrementar += "\nMUL BX ";
                        asmIncrementar += "\nMOV " + Variable + ", AX";
                    }
                    else 
                    {
                        asmIncrementar += "\nMOV BL, " + Variable;
                        asmIncrementar += "\nMUL BL ";
                        asmIncrementar += "\nMOV " + Variable + ", AL";
                    }
                }
                if (evaluacion)
                {
                    variableModificada *= resultado;
                }
            }
            else if (getContenido() == "/=")
            {
                match("/=");
                Expresion(imprimir);
                resultado = stack.Pop();
                if (imprimir)
                {
                    asmIncrementar = "POP BX";
                    if(getTipo(Variable) > Semantica.Variable.TipoDato.Char)
                    {
                        asmIncrementar += "\nMOV AX, " + Variable;
                        asmIncrementar += "\nDIV BX ";
                        asmIncrementar += "\nMOV " + Variable + ", AX";
                    }
                    else
                    {
                        asmIncrementar += "\nMOV AL, " + Variable;
                        asmIncrementar += "\nDIV BL ";
                        asmIncrementar += "\nMOV " + Variable + ", AL";
                    }
                }
                if (evaluacion)
                {
                    variableModificada /= resultado;
                }
            }else if (getContenido() == "%=")
            {
                match("%=");
                Expresion(imprimir);
                resultado = stack.Pop();
                if (imprimir)
                {
                    asmIncrementar = "POP BX";
                    if(getTipo(Variable) > Semantica.Variable.TipoDato.Char)
                    {
                        asmIncrementar += "\nMOV AX, " + Variable;
                        asmIncrementar += "\nDIV BX ";
                        asmIncrementar += "\nMOV " + Variable + ", DX";
                    }
                    else
                    {
                        asmIncrementar += "\nMOV AL, " + Variable;
                        asmIncrementar += "\nDIV BL ";
                        asmIncrementar += "\nMOV " + Variable + ", DL";
                    }
                }
                if (evaluacion)
                {
                    variableModificada %= resultado;
                }
            }
            
            if(getTipo(Variable) < dominante)
            {
                throw new Error("Error de semantica no podemos asignar un <" + dominante + "> a un <" + getTipo(Variable) + "> en linea: " + linea, log);
            }
            return variableModificada;
        }

        //Switch -> switch (Expresion) {Lista de casos} | (default: )
        private void Switch(bool evaluacion, bool imprimir)
        {
            match("switch");
            match("(");
            Expresion(imprimir);
            stack.Pop();
            if(imprimir)
            {
                asm.WriteLine("POP AX");
            }
            match(")");
            match("{");
            ListaDeCasos(evaluacion, imprimir);
            if (getContenido() == "default")
            {
                match("default");
                match(":");
                if (getContenido() == "{")
                {
                    BloqueInstrucciones(evaluacion, imprimir);
                }
                else
                {
                    Instruccion(evaluacion, imprimir);
                }
            }
            match("}");
        }

        //ListaDeCasos -> case Expresion: listaInstruccionesCase (break;)? (ListaDeCasos)?
        private void ListaDeCasos(bool evaluacion, bool imprimir)
        {
            match("case");
            Expresion(imprimir);
            stack.Pop();
            if(imprimir)
            {
                asm.WriteLine("POP AX");
            }
            match(":");
            ListaInstruccionesCase(evaluacion, imprimir);
            if (getContenido() == "break")
            {
                match("break");
                match(";");
            }
            if (getContenido() == "case")
            {
                ListaDeCasos(evaluacion, imprimir);
            }
        }

        //Condicion -> Expresion operador relacional Expresion
        private bool Condicion(string etiqueta, bool imprimir)
        {
            float e1, e2;
            Expresion(imprimir);
            string operador = getContenido();
            match(Tipos.OperadorRelacional);
            Expresion(imprimir);
            e2 = stack.Pop();
            if(imprimir)
            {
                asm.WriteLine("POP BX");
                asm.WriteLine("POP AX");
                asm.WriteLine("CMP AX, BX");
            }
            e1 = stack.Pop();
            //asm.WriteLine("POP AX");
            //asm.WriteLine("CMP AX, BX");
            switch (operador)
            {
                case "==":
                    if(imprimir)
                    {
                        asm.WriteLine("JNE " + etiqueta);
                    }
                    return e1 == e2;
                case ">":
                    if(imprimir)
                    {
                        asm.WriteLine("JLE " + etiqueta);
                    }
                    return e1 > e2;
                case ">=":
                    if(imprimir)
                    {
                        asm.WriteLine("JL " + etiqueta);
                    }
                    return e1 >= e2;
                case "<":
                    if(imprimir)
                    {
                        asm.WriteLine("JGE " + etiqueta);
                    }
                    return e1 < e2;
                case "<=":
                    if(imprimir)
                    {
                        asm.WriteLine("JG " + etiqueta);
                    }
                    return e1 <= e2;
                default:
                    if(imprimir)
                    {
                        asm.WriteLine("JE " + etiqueta);
                    }
                    return e1 != e2;
            }
        }

        //If -> if(Condicion) bloque de instrucciones (else bloque de instrucciones)?
        private void If(bool evaluacion, bool imprimir)
        {
            if(imprimir)
            {
                cIf++;
            }
            string etiquetaIf = "if" + cIf;
            string etiquetaElse = "else" + cIf;
            match("if");
            match("(");
            bool validarIf = Condicion(etiquetaIf, imprimir);
            if (!evaluacion)
            {
                validarIf = evaluacion;
            }
            match(")");
            if (getContenido() == "{")
            {
                BloqueInstrucciones(validarIf, imprimir);
            }
            else
            {
                Instruccion(validarIf, imprimir);
            }
            if(imprimir)
            {
                asm.WriteLine("JMP " + etiquetaElse);
                asm.WriteLine(etiquetaIf + ":");
            }
            if (getContenido() == "else")//Requerimiento 4
            {
                match("else");
                if (getContenido() == "{")
                {
                    if (evaluacion)
                    {
                        BloqueInstrucciones(!validarIf, imprimir);
                    }
                    else
                    {
                        BloqueInstrucciones(evaluacion, imprimir);
                    }
                }
                else
                {
                    if (evaluacion)
                    {
                        Instruccion(!validarIf, imprimir);
                    }
                    else
                    {
                        Instruccion(evaluacion, imprimir);
                    }
                }
            }
            if(imprimir)
            {
                asm.WriteLine(etiquetaElse + ":");
            }
        }

        //Printf -> printf(cadena|expresion);
        private void Printf(bool evaluacion, bool imprimir)
        {
            string cadena = "";
            match("printf");
            match("(");
            if (getClasificacion() == Tipos.Cadena)
            {
                setContenido(getContenido().Replace("\\t", "    "));
                setContenido(getContenido().Replace("\"", string.Empty));
                cadena = getContenido();
                setContenido(getContenido().Replace("\\n", "\n"));
                if (evaluacion)
                {
                    Console.Write(getContenido());
                }
                if(imprimir)
                {
                    if(cadena.Contains("\\n"))
                    {
                        saltosASM(cadena);
                    }
                    else
                    {
                        asm.WriteLine("PRINT \'" + cadena + "\'");
                    }
                }
                match(Tipos.Cadena);
            }
            else
            {
                Expresion(imprimir);
                float resultado = stack.Pop();
                if (evaluacion)
                {
                    Console.Write(resultado);
                }
                if(imprimir)
                {
                    asm.WriteLine("POP AX");
                    asm.WriteLine("CALL PRINT_NUM");
                }
                //Escribir variables
            }
            match(")");
            match(";");
        }

        //Scanf -> scanf(cadena,&identificador);
        private void Scanf(bool evaluacion, bool imprimir)
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
                if(imprimir)
                {
                    asm.WriteLine("CALL scan_num");
                    asm.WriteLine("MOV " + getContenido() + ", CX");
                    asm.WriteLine("PRINTN \'\'");
                }
            }
            match(Tipos.Identificador);
            match(")");
            match(";");
        }



        //Expresion -> Termino MasTermino
        private void Expresion(bool imprimir)
        {
            Termino(imprimir);
            MasTermino(imprimir);
        }
        //MasTermino -> (OperadorTermino Termino)?
        private void MasTermino(bool imprimir)
        {
            if (getClasificacion() == Tipos.OperadorTermino)
            {
                string operador = getContenido();
                match(Tipos.OperadorTermino);
                Termino(imprimir);
                float n1 = stack.Pop();
                float n2 = stack.Pop();
                if(imprimir) 
                {
                    asm.WriteLine("POP BX");
                    asm.WriteLine("POP AX");
                } 
                switch (operador)
                {
                    case "+":
                        stack.Push(n2 + n1);
                        if(imprimir) 
                        {
                            asm.WriteLine("ADD AX, BX");
                            asm.WriteLine("PUSH AX");
                        }
                        break;
                    case "-":
                        stack.Push(n2 - n1);
                        if(imprimir)
                        {
                            asm.WriteLine("SUB AX, BX");
                            asm.WriteLine("PUSH AX");
                        } 
                        break;
                }
            }
        }
        //Termino -> Factor PorFactor
        private void Termino(bool imprimir)
        {
            Factor(imprimir);
            PorFactor(imprimir);
        }
        //PorFactor -> (OperadorFactor Factor)? 
        private void PorFactor(bool imprimir)
        {
            if (getClasificacion() == Tipos.OperadorFactor)
            {
                string operador = getContenido();
                match(Tipos.OperadorFactor);
                Factor(imprimir);
                float n1 = stack.Pop();
                float n2 = stack.Pop();
                if(imprimir) 
                {
                    asm.WriteLine("POP BX");
                    asm.WriteLine("POP AX");
                } 
                //Requerimiento 1.a
                switch (operador)
                {
                    case "*":
                        stack.Push(n2 * n1);
                        if(imprimir)
                        {
                            asm.WriteLine("MUL BX");
                            asm.WriteLine("PUSH AX");
                        }
                        break;
                    case "/":
                        stack.Push(n2 / n1);
                        if(imprimir)
                        {
                            asm.WriteLine("DIV BX");
                            asm.WriteLine("PUSH AX");
                        }
                        break;
                    case "%": 
                        stack.Push(n2 % n1);
                        if(imprimir)
                        {
                            asm.WriteLine("DIV BX");
                            asm.WriteLine("PUSH DX");
                        }
                        break;
                }
            }
        }
        //Factor -> numero | identificador | (Expresion)
        private void Factor(bool imprimir)
        {
            if (getClasificacion() == Tipos.Numero)
            {
                if (dominante < evaluaNumero(float.Parse(getContenido())))
                {
                    dominante = evaluaNumero(float.Parse(getContenido()));
                }
                stack.Push(float.Parse(getContenido()));
                if(imprimir)
                {
                    asm.WriteLine("MOV AX, " + getContenido());
                    asm.WriteLine("PUSH AX");
                }
                match(Tipos.Numero);
            }
            else if (getClasificacion() == Tipos.Identificador)
            {
                //Requerimiento 3: a) Agregar los tipos de identificador
                stack.Push(getValor(getContenido()));
                if (!existeVariable(getContenido()))
                {
                    throw new Error("Error: No existe la variable " + getContenido() + " en linea: " + linea, log);
                }
                if (dominante < getTipo(getContenido())) //Requisito 1
                {
                    dominante = getTipo(getContenido());
                }
                if(getTipo(getContenido()) == Variable.TipoDato.Char)
                {
                    if(imprimir)
                    {
                        asm.WriteLine("MOV AL, " + getContenido());
                        asm.WriteLine("MOV AH, 0");
                    }
                }
                else 
                {
                    if(imprimir)
                    {
                        asm.WriteLine("MOV AX, " + getContenido());
                    }
                }
                if(imprimir)    
                    asm.WriteLine("PUSH AX");
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
                Expresion(imprimir);
                match(")");
                if (huboCasteo)
                {
                    dominante = casteo;
                    float valor = stack.Pop();
                    stack.Push(convierte(valor, casteo));
                }
            }
        }
    }
}