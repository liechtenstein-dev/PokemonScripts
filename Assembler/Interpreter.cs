using System;
using System.Linq;
using System.Text.RegularExpressions;

// Disable ReSharper InconsistencyNaming
public class Interpreter : IDisposable
{
    public BaseRegisters baseRegisters = new BaseRegisters();

    public ushort[] GetRegisters()
    {
        return baseRegisters.Registers.Values.ToArray();
    }

    public ushort GetRegister(string reg)
    {
        return baseRegisters.Registers[reg];
    }

    public void Run(string program)
    {
        var lineIndex = 0;
        var lines = program.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        lines = lines.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
        var labels = baseRegisters.Labels;
        var registers = baseRegisters.Registers;
        baseRegisters.Registers["IP"] = 0;
        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i].Trim();
            if (!line.EndsWith(":")) continue;
            labels[line.Substring(0, line.Length - 1)] = (ushort)i;
        }

        while (lineIndex < lines.Length)
        {
            var line = lines[lineIndex].Trim();
            
            if (!string.IsNullOrEmpty(line) && !line.EndsWith(":") && !labels.ContainsKey(line) && !baseRegisters.JumpAvailable)
            {
                Execute(line);
            }
            
            if (!string.IsNullOrEmpty(line) && !line.EndsWith(":") && baseRegisters.JumpAvailable)
            {
                Execute(line);
            }
            
            baseRegisters.Registers["IP"]++;
            lineIndex = baseRegisters.Registers["IP"];
        }
    }

    public void Execute(string line)
    {
        var parts = line.Split(' ');
        var inst = parts[0];
        var operand1 = parts.Length > 1 ? parts[1] : "";
        var operand2 = parts.Length > 2 ? parts[2] : "";
        operand1 = Regex.Replace(operand1, @",.*|;.*", "");
        operand2 = Regex.Replace(operand2, @",.*|;.*", "").ToUpper();

        var instructionRequirementToParams = new InstructionRequirement(inst,
            operand1, operand2, baseRegisters);
        var instructionSet = new Instruction(instructionRequirementToParams);
        instructionRequirementToParams = instructionSet.GetValuesCompleted();
        baseRegisters = instructionRequirementToParams.BaseRegs;
    }
    
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        GC.Collect();
    }

    ~Interpreter()
    {
        Dispose();
    }
}