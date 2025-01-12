grammar Expressions;

// defines
WHILE: 'while';
FOR: 'for';
FOREACH: 'foreach';
INTEGER: '-'? [0-9]+;
DOUBLE: '-'? [0-9]+ '.' [0-9]+;
STRING: '"' (~["\r\n])* '"';
BOOL: 'true' | 'false';
CHAR: '\'' . '\'';
NULL: 'null';
WS: [ \t\r\n]+ -> skip;
DATA_TYPES: ('[' ']'DATA_TYPES) | 'i16' | 'i32' | 'i64' | 'string' | 'bool' | 'char' | 'double' | 'any' | 'void' | '{}';
IDENTIFIER: [a-zA-Z_][a-zA-Z0-9_]*;
REFERENCE: '&';
USE_IDENTIFIERS: 'disable-type-checking';

// grammar
program: line* EOF;
line:  packageDeclaration | usingDirective | structBlock | directive | statement | ifBlock | whileBlock | forBlock | foreachBlock | functionDeclaration | return;
block: '{' line* '}';

structBody: '{' structLine* '}';
structLine: variableDefine ';'; 

directive: '#' 'use' USE_IDENTIFIERS;

multiplyOp: '*' | '/' | '%';
addOp: '+' | '-';
compareOp: '==' | 'is' | '!=' | 'is not' | '>' | '<' | '>=' | '<=';
boolOp: 'and' | 'or' | 'xor';
accessLevels: 'pub';

arrayLiteral: '[' (expression (',' expression)*)? ']' ;
arrayAccessor: '[' (expression) ']';

dictionaryEntry: (STRING/* | IDENTIFIER | INTEGER*/) ':' expression;
dictionaryLiteral: '{' (dictionaryEntry (',' dictionaryEntry)*)? '}';

arrayLength: 'len' '(' expression ')';

break: 'break';
continue: 'continue';
return: 'return' expression;

expression
    : constant                               #constantExpr
    | IDENTIFIER                             #identifierExpr
    | functionCall                           #functionCallExpr
    | '(' expression ')'                     #parenthesizedExpr
    | '!' expression                         #negationExpr
    | expression arrayAccessor               #arrayIndexExpr // order is neccesarry
    | arrayLiteral DATA_TYPES                #arrayLiteralExpr // array literal -> [] any
    | expression multiplyOp expression       #multiplyExpr
    | expression addOp expression            #addExpr
    | expression compareOp expression        #compareExpr
    | expression boolOp expression           #boolExpr
    | '(' DATA_TYPES ')' expression          #castExpr
    | arrayLength                            #arrayLengthExpr
    | dictionaryLiteral                      #dictionaryLiteralExpr // x = {}
    | instanceCreate                         #instanceCreateExpr
    | instancePropCall                       #instancePropCallExpr
    ;

packageDeclaration: 'package' IDENTIFIER;
usingDirective: 'using' IDENTIFIER;

variableDeclaration: 'var' IDENTIFIER '=' REFERENCE? expression;
variableDefine: 'var' IDENTIFIER DATA_TYPES;
functionDeclaration: (accessLevels)? 'funky' IDENTIFIER '(' (parameterList)? ')' DATA_TYPES block;

parameterList: parameter (',' parameter)*;
parameter: IDENTIFIER DATA_TYPES;

constant: INTEGER | DOUBLE | STRING | BOOL | CHAR | NULL;

assignment: IDENTIFIER arrayAccessor? '=' expression;
operationAssignment: IDENTIFIER (addOp|multiplyOp) '=' expression;
selfOperation: ('++'|'--'|'**'|'//')? IDENTIFIER ('++'|'--'|'**'|'//')?;

functionCall
    : IDENTIFIER '(' (expression (',' expression)*)? ')'                    # simpleFunctionCall
    | IDENTIFIER '::' IDENTIFIER '(' (expression (',' expression)*)? ')'    # namespacedFunctionCall
    | IDENTIFIER '.' IDENTIFIER '(' (expression (',' expression)*)? ')'     # methodCall
    ;

statement: (variableDeclaration | assignment | operationAssignment | selfOperation | functionCall | instancePropAssign | break | continue | return) ';';

// Blocks
ifBlock: 'if' '('? expression ')'? block ('else' elseIfBlock)?;
elseIfBlock: block | ifBlock;

whileBlock: WHILE '('? expression ')'? block;
forBlock: FOR '('? (variableDeclaration | assignment)? ';' expression? ';' (selfOperation)? ')'? block;
foreachBlock: FOREACH '(' 'var' IDENTIFIER 'in' expression ')' block;

structBlock: accessLevels? 'struct' IDENTIFIER structBody;

instanceCreate: 'new' IDENTIFIER '(' (expression (',' expression)*)? ')';
instancePropAssign: IDENTIFIER '.' IDENTIFIER '=' expression;
instancePropCall: IDENTIFIER '.' IDENTIFIER;