using System;

public class Functions : IDisposable
{
    public Interpreter interpreter { get; set; }
    /*
     *  Valor: Tomara un valor que sera el numero con el que se operara
     *  Increment: Sera la cantidad de bits que se van a desplazar de ese número 
     *  Interpreter: el objeto de Interpretador de assembler utlizado para que se ejecute el programa
     * 
     */
    public void SettingValues(int valor)
    {
        interpreter.Run(@$"
            MOV AX, {valor}D
        ");
    }
    
    public int RotateNumber(int increment)
    {
        interpreter.Run(@$"
        MOV CX, {increment}d ; Cantidad de bits a desplazar
        ROR AX, CL ; GIRO DE BITS
        MOV BX, 000Fh ; Máscara para conseguir los últimos 4 bits
        XOR AX, BX ; Aplico la máscara a AX
        CMP AX, 15D ; Compara AX con el valor 15
        JE set_bx ; Si es igual a 15, salta a la etiqueta set_bx
        JNE multiply ; Si no es igual a 15, salta a la etiqueta multiply
        set_bx:
        MOV BX, 1D ; Establece el valor 1 en el registro BX
        MUL BX ; Multiplica el contenido de AX por el valor en BX (resultado en AX)
        JMP end ; Salta al final para evitar la parte de abajo
        multiply:
        MOV BX, 0D ; Puedes elegir otro valor (por ejemplo, 2)
    end:
    ");
        return Convert.ToUInt16(interpreter.GetRegister("BX"));
    }

    public int CalculateAttackValue(int attackerLevel)
    {
        interpreter.Run(@$"
        MOV AX, {attackerLevel}D
        SHL AX, 1D
        ADD AX, 10D
        MOV BX, 5D
        MUL BX ; Multiplica el contenido de AX por el valor en BX (resultado en AX)
        SUB AX, 10D 
    ");
        return Convert.ToUInt16(interpreter.GetRegister("AX"));
    }

    public void Dispose()
    {
        interpreter?.Dispose();
    }
}