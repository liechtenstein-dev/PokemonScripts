/**
 * Estructura: InstructionRequirement
 * Descripción: Representa los requisitos de una instrucción en lenguaje ensamblador.
 */
public struct InstructionRequirement
{
    /**
     * Campo: NameOfInstruction
     * Descripción: Almacena el nombre de la instrucción.
     * Tipo: string
     */
    public string NameOfInstruction;

    /**
     * Campo: StrOperand
     * Descripción: Almacena el primer operando de la instrucción (si existe).
     * Tipo: string
     */
    public string StrOperand;

    /**
     * Campo: SecStrOperand
     * Descripción: Almacena el segundo operando de la instrucción (si existe).
     * Tipo: string
     */
    public string SecStrOperand;

    /**
     * Campo: BaseRegs
     * Descripción: Almacena una instancia de la estructura BaseRegisters, que contiene los registros y memoria base necesarios para el procesamiento de la instrucción.
     * Tipo: BaseRegisters
     */
    public BaseRegisters BaseRegs;

    #region Constructor

    /**
     * Constructor: InstructionRequirement
     * Descripción: Inicializa una nueva instancia de la estructura InstructionRequirement con el nombre de la instrucción y los registros base proporcionados.
     * Parámetros:
     *   - nameOfInstruction: El nombre de la instrucción.
     *   - baseRegs: Una instancia de BaseRegisters que contiene los registros y memoria base necesarios para el procesamiento de la instrucción.
     */
    public InstructionRequirement(string nameOfInstruction, BaseRegisters baseRegs)
    {
        NameOfInstruction = nameOfInstruction;
        StrOperand = string.Empty;
        SecStrOperand = string.Empty;
        BaseRegs = baseRegs;
    }

    /**
     * Constructor: InstructionRequirement
     * Descripción: Inicializa una nueva instancia de la estructura InstructionRequirement con el nombre de la instrucción, el primer operando y los registros base proporcionados.
     * Parámetros:
     *   - nameOfInstruction: El nombre de la instrucción.
     *   - strOperand: El primer operando de la instrucción.
     *   - baseRegs: Una instancia de BaseRegisters que contiene los registros y memoria base necesarios para el procesamiento de la instrucción.
     */
    public InstructionRequirement(string nameOfInstruction, string strOperand, BaseRegisters baseRegs)
    {
        NameOfInstruction = nameOfInstruction;
        StrOperand = strOperand;
        SecStrOperand = string.Empty;
        BaseRegs = baseRegs;
    }

    /**
     * Constructor: InstructionRequirement
     * Descripción: Inicializa una nueva instancia de la estructura InstructionRequirement con el nombre de la instrucción, el primer y segundo operando, y los registros base proporcionados.
     * Parámetros:
     *   - nameOfInstruction: El nombre de la instrucción.
     *   - strOperand: El primer operando de la instrucción.
     *   - secStrOperand: El segundo operando de la instrucción.
     *   - baseRegs: Una instancia de BaseRegisters que contiene los registros y memoria base necesarios para el procesamiento de la instrucción.
     */
    public InstructionRequirement(string nameOfInstruction, string strOperand, string secStrOperand, BaseRegisters baseRegs)
    {
        NameOfInstruction = nameOfInstruction;
        StrOperand = strOperand;
        SecStrOperand = secStrOperand;
        BaseRegs = baseRegs;
    }
    #endregion
}
