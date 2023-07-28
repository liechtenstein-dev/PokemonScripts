using System;
using System.Collections.Generic;

public class BaseRegisters
{
    public Dictionary<string, ushort> Registers;
    public Dictionary<ushort, ushort> Memory;
    public Dictionary<string, ushort> Labels;
    public Boolean JumpAvailable;
    public ushort[] Stack;

    public BaseRegisters(Dictionary<string, ushort> registers, Dictionary<ushort, ushort> memory, Dictionary<string, ushort> labels, ushort[] stack)
    {
        Registers = registers;
        Memory = memory;
        Labels = labels;
        Stack = stack;
    }
    public BaseRegisters()
    {
        Registers = new Dictionary<string, ushort>()
        {
            {"AX", 0},
            {"BX", 0},
            {"CX", 0},
            {"DX", 0},
            // POINTERS
            {"SP", 0},
            {"BP", 0},
            {"SI", 0},
            {"DI", 0},
            {"IP", 0},
            // SEGMENTS
            {"CS", 0},
            {"DS", 0},
            {"ES", 0},
            {"SS", 0},
            // FLAGS
            {"CF", 0},
            {"PF", 0},
            {"AF", 0},
            {"ZF", 0},
            {"SF", 0},
            {"TF", 0},
            {"IF", 0},
            {"DF", 0},
            {"OF", 0},
        };
        Memory = new Dictionary<ushort, ushort>();
        Labels = new Dictionary<string, ushort>();
        JumpAvailable = false;
        Stack = new ushort[65535];
    }
    
    public ushort TransformLh(string reg)
    {
        // Si dice AH-AL y asi con los 4 registros que retorne el reg completo
        if (reg.StartsWith('A'))
            return Registers["AX"];
        if (reg.StartsWith('B'))
            return Registers["BX"];
        if (reg.StartsWith('C'))
            return Registers["CX"];
        return reg.StartsWith('D') ? Registers["DX"] : (ushort)0;
    }
}