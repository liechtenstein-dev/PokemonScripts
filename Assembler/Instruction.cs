using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public class Instruction
{
    #region Constructors and Properties
    private InstructionRequirement innerInstruction;
    public Instruction(InstructionRequirement instruction)
    {
        innerInstruction = instruction;
        if(instruction.StrOperand == string.Empty && instruction.SecStrOperand == string.Empty)
        {
            NoOperandInstruction();
        }
        if (instruction.SecStrOperand == string.Empty && instruction.StrOperand != string.Empty)
        {
            var operand = ParseOperand(instruction.StrOperand);
            OneOperandInstruction(operand, instruction.StrOperand);
        }
        if (instruction.StrOperand != string.Empty && instruction.SecStrOperand != string.Empty)
        {
            var operand = ParseOperand(instruction.StrOperand);
            var secondOperand = ParseOperand(instruction.SecStrOperand);
            TwoOperandInstruction(operand, secondOperand, instruction.StrOperand);
        }
    }
    #endregion

    #region ParseOperand
    private ushort ParseOperand(string operand)
    {
        var registers = innerInstruction.BaseRegs.Registers;
        var memory = innerInstruction.BaseRegs.Memory;
        var labels = innerInstruction.BaseRegs.Labels;
        // SI ES PARTE ALTA
        if (Regex.IsMatch(operand, @"^[A-Za-z]H$|^H[A-Za-z]$"))
        {
            return (byte)(innerInstruction.BaseRegs.TransformLh(operand) >> 8);
        }

        // SI ES PARTE BAJA
        if (Regex.IsMatch(operand, @"^[A-Za-z]L$|^L[A-Za-z]$"))
        {
            return (byte)(innerInstruction.BaseRegs.TransformLh(operand) & 0xFF);
        }

        // SI ES EL REGISTRO ENTERO
        if (Regex.IsMatch(operand, @"^[A-Za-z]X$|^X[A-Za-z]$"))
        {
            return (ushort)(registers[operand] & 0xFFFF);
        }

        // SI ES UN BINARIO            
        if (operand.EndsWith("B"))
        {
            operand = operand.Substring(0, operand.Length - 1);
            return Convert.ToUInt16(operand, 2);
        }

        // SI ES HEXA
        if (operand.EndsWith('H'))
        {
            operand = operand.Substring(0, operand.Length - 1);
            return Convert.ToUInt16(operand, 16);
        }

        // SI ES DECIMAL
        if (operand.EndsWith('D'))
        {
            operand = operand.Substring(0, operand.Length - 1);
            return Convert.ToUInt16(operand, 10);
        }

        if (operand.StartsWith("[") && operand.EndsWith("]"))
        {
            string memReg1 = operand.Substring(1, operand.Length - 2);
            ushort offset1 = registers[memReg1];
            return memory[offset1];
        }

        if (operand.StartsWith("[") && operand.Contains("+") && operand.EndsWith("]"))
        {
            string[] parts = operand.Substring(1, operand.Length - 2).Split('+');
            string memReg1 = parts[0];
            ushort offset1 = registers[memReg1];
            ushort offset2 = ParseOperand(parts[1]);
            ushort address = (ushort)(offset1 + offset2);
            return memory[address];
        }

        if (operand.StartsWith("[") && operand.Contains("-") && operand.EndsWith("]"))
        {
            string[] parts = operand.Substring(1, operand.Length - 2).Split('-');
            string memReg1 = parts[0];
            ushort offset1 = registers[memReg1];
            ushort offset2 = ParseOperand(parts[1]);
            ushort address = (ushort)(offset1 - offset2);
            return memory[address];
        }

        if (operand.StartsWith("[") && operand.EndsWith("]"))
        {
            string memReg1 = operand.Substring(1, operand.Length - 2);
            ushort offset1 = registers[memReg1];
            return memory[offset1];
        }

        if (labels.ContainsKey(operand))
        {
            return labels[operand];
        }
        else
        {
            throw new InvalidOperationException("Invalid operand: " + operand);
        }
    }
    #endregion 
    #region OperandInstructions
    private void TwoOperandInstruction(ushort operand, ushort second_operand, string str_operand)
    {
        var registers = innerInstruction.BaseRegs.Registers;
        var name = innerInstruction.NameOfInstruction;
        
        Dictionary<string, Action<string>> instruction = new()
        {
            {
                "MOV", (str_operand) => registers[str_operand] = second_operand
            },
            {
                "ADD", (str_operand) => registers[str_operand] = (ushort)(operand + second_operand)
            },
            {
                "SUB", (str_operand) => registers[str_operand] = (ushort)(operand - second_operand)
            },
            {
                "MUL", (str_operand) => registers[str_operand] = (ushort)(operand * second_operand)
            },
            {
                "DIV", (str_operand) => registers[str_operand] = (ushort)(operand / second_operand)
            },
            {
                "AND", (str_operand) => registers[str_operand] = (ushort)(operand & second_operand)
            },
            {
                "OR", (str_operand) => registers[str_operand] = (ushort)(operand | second_operand)
            },
            {
                "XOR", (str_operand) => registers[str_operand] = (ushort)(operand ^ second_operand)
            },
            {
                "SHL", (str_operand) => registers[str_operand] = (ushort)(operand << second_operand)
            },
            {
                "SHR", (str_operand) => registers[str_operand] = (ushort)(operand >> second_operand)
            },
            {
                "ROR",
                (str_operand) => registers[str_operand] =
                    (ushort)((operand >> second_operand) | (operand << (16 - second_operand)))
            },
            {
                "ROL",
                (str_operand) => registers[str_operand] =
                    (ushort)((operand << second_operand) | (operand >> (16 - second_operand)))
            },
        };

        Dictionary<string, Action> no_param_instruction = new Dictionary<string, Action>()
        {
            {
                "CMP", () =>
                {
                    registers["ZF"] = (ushort)((operand - second_operand) == 0 ? 1 : 0);
                    registers["SF"] = (ushort)((operand & 0x8000) == (second_operand & 0x8000) ? 1 : 0);
                    registers["OF"] = (ushort)((operand & 0x8000) != (second_operand & 0x8000) ? 1 : 0);
                    registers["AF"] = (ushort)((operand & 0x0F) == (second_operand & 0x0F) ? 1 : 0);
                    //  registers["PF"] = (ushort)((((operand & 0x0F) + (second_operand & 0x0F)) & 0x10) ? 1 : 0);
                    registers["CF"] = (ushort)((operand - second_operand) < 0 ? 1 : 0);
                }
            }
        };

        var keys = instruction.Keys;
        if (keys.Contains(name))
        {
            instruction[name](str_operand);
            return;
        }
        if(no_param_instruction.ContainsKey(name))
        {
            no_param_instruction[name]();
            return;
        }
        throw new InvalidOperationException("Invalid instruction: " + name);
    }
    private void OneOperandInstruction(ushort operand, string str_operand)
    {
        var stack = innerInstruction.BaseRegs.Stack;
        var registers = innerInstruction.BaseRegs.Registers;
        var name = innerInstruction.NameOfInstruction;
        var jumpable = innerInstruction.BaseRegs.JumpAvailable;
        Dictionary<string, Action<string>> instruction = new()
        {
                /*
            {
                "INT", (str_operand) =>
                {
                    operand = Convert.ToUInt16(str_operand);
                    if (operand == 21 && ParseOperand("AH").ToString() == "4Ch")
                    {
                        Interpreter.Dispose();
                    }
                }
            },
                */
            {
                "JMP", (str_operand) =>
                {
                    stack[registers["SP"]] = registers["IP"];
                    registers["SP"] = (ushort)(registers["SP"] - 2);
                    registers["IP"] = ParseOperand(str_operand);
                    jumpable = true;
                }
            },
            {
                "JZ", (str_operand) =>
                {
                    if (registers["ZF"] == 1)
                    {
                        registers["SP"] = (ushort)(registers["SP"] - 2);
                        registers["IP"] = ParseOperand(str_operand);
                        jumpable = true;
                    }
                }
            },
            {
                "JNZ", (str_operand) =>
                {
                    if (registers["ZF"] == 0)
                    {
                        stack[registers["SP"]] = registers["IP"];
                        registers["SP"] = (ushort)(registers["SP"] - 2);
                        registers["IP"] = ParseOperand(str_operand);
                        jumpable = true;
                    }
                }
            },
            {
                "JE", (str_operand) =>
                {
                    if (registers["ZF"] == 1)
                    {
                        stack[registers["SP"]] = registers["IP"];
                        registers["SP"] = (ushort)(registers["SP"] - 2);
                        registers["IP"] = ParseOperand(str_operand);
                        jumpable = true;
                    }
                }
            },
            {
                "JNE", (str_operand) =>
                {
                    if (registers["ZF"] == 0)
                    {
                        stack[registers["SP"]] = registers["IP"];
                        registers["SP"] = (ushort)(registers["SP"] - 2);
                        registers["IP"] = ParseOperand(str_operand);
                        jumpable = true;
                    }
                }
            },
            {
                "JG", (str_operand) =>
                {
                    if (registers["SF"] == registers["OF"])
                    {
                        stack[registers["SP"]] = registers["IP"];
                        registers["SP"] = (ushort)(registers["SP"] - 2);
                        registers["IP"] = ParseOperand(str_operand);
                        jumpable = true;
                    }
                }
            },
            {
                "JGE", (str_operand) =>
                {
                    if (registers["SF"] != registers["OF"])
                    {
                        stack[registers["SP"]] = registers["IP"];
                        registers["SP"] = (ushort)(registers["SP"] - 2);
                        registers["IP"] = ParseOperand(str_operand);
                        jumpable = true;
                    }
                }
            },
            {
                "JL", (str_operand) =>
                {
                    if (registers["SF"] == 0)
                    {
                        stack[registers["SP"]] = registers["IP"];
                        registers["SP"] = (ushort)(registers["SP"] - 2);
                        registers["IP"] = ParseOperand(str_operand);
                        jumpable = true;
                    }
                }
            },
            {
                "JLE", (str_operand) =>
                {
                    if (registers["SF"] != 0)
                    {
                        stack[registers["SP"]] = registers["IP"];
                        registers["SP"] = (ushort)(registers["SP"] - 2);
                        registers["IP"] = ParseOperand(str_operand);
                        jumpable = true;
                    }
                }
            },
            {
                "PUSH", (str_operand) =>
                {
                    // STACK[SP] = OPERAND
                    // SP = SP - 2
                    stack[registers["SP"]] = operand;
                    registers["SP"] = (ushort)(registers["SP"] - 2);
                }
            },
            {
                "POP", (str_operand) =>
                {
                    // SP = SP + 2
                    // OPERAND = stack[SP]
                    // stack[SP] = 0 
                    registers["SP"] = (ushort)(registers["SP"] + 2);
                    operand = stack[registers["SP"]];
                    stack[registers["SP"]] = 0;
                }
            },
            {
                "INC", (str_operand) => { registers[str_operand] = (ushort)(operand + 1); }
            },
            {
                "DEC", (str_operand) =>
                {
                    ushort operand = ParseOperand(str_operand);
                    registers[str_operand] = (ushort)(operand - 1);
                }
            },
            {
                "NOT", (str_operand) =>
                {
                    ushort operand = ParseOperand(str_operand);
                    registers[str_operand] = (ushort)(~operand);
                }
            },
            {
                "CALL", (str_operand) =>
                {
                    ushort operand = ParseOperand(str_operand);
                    stack[registers["SP"]] = registers["IP"];
                    registers["SP"] = (ushort)(registers["SP"] - 2);
                    registers["IP"] = operand;
                }
            },
            {
                "MUL", (str_operand) => registers["AX"] = (ushort)(registers[str_operand] * registers["AX"])
            },
        };
        
        if (instruction.ContainsKey(name))
        {
            instruction[name](str_operand);
        }
        else
        { 
            throw new InvalidOperationException($"Invalid One Operand Instruction");
        }
    }
    private void NoOperandInstruction()
    {
        var registers = innerInstruction.BaseRegs.Registers;
        var stack = innerInstruction.BaseRegs.Stack;
        var jumpable = innerInstruction.BaseRegs.JumpAvailable;
        Dictionary<string, Action> instruction = new Dictionary<string, Action>()
        {
            {
                "RET", () =>
                {
                    registers["SP"] = (ushort)(registers["SP"] + 2);
                    registers["IP"] = stack[registers["SP"]];
                    jumpable = false;
                }
            },
            {
                "RETI", () =>
                {
                    registers["SP"] = (ushort)(registers["SP"] + 2);
                    registers["IP"] = stack[registers["SP"]];
                    registers["IF"] = 1;
                }
            }
        };
        if(instruction.ContainsKey(innerInstruction.NameOfInstruction))
        {
            instruction[innerInstruction.NameOfInstruction]();
        }
        else
        { 
            throw new InvalidOperationException($"Invalid No Operand Instruction");
        }
    }
    public InstructionRequirement GetValuesCompleted()
    {
        return innerInstruction;
    }
    #endregion
    
}