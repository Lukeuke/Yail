grammar Expressions;

// defines
WHILE: 'while';
INTEGER: [0-9]+;
DOUBLE: [0-9]+ '.' [0-9]+;
STRING: '"' (~["\r\n])* '"';
BOOL: 'true' | 'false';
CHAR: '\'' . '\'';
NULL: 'null';
WS: [ \t\r\n]+ -> skip;
DATA_TYPES: 'i16' | 'i32' | 'i64' | 'string' | 'bool' | 'char' | 'double' | 'any';
IDENTIFIER: [a-zA-Z_][a-zA-Z0-9_]*;

// grammar
program: line* EOF;
line: statement | ifBlock | whileBlock | functionDeclaration | return;
block: '{' line* '}';

multiplyOp: '*' | '/' | '%';
addOp: '+' | '-';
compareOp: '==' | '!=' | '>' | '<' | '>=' | '<=';
boolOp: 'and' | 'or' | 'xor'; // TODO:

break: 'break';
return: 'return' expression;

expression
    : constant                               #constantExpr
    | IDENTIFIER                             #identifierExpr
    | functionCall                           #functionCallExpr
    | '(' expression ')'                     #parenthesizedExpr
    | '!' expression                         #negationExpr // TODO:
    | expression multiplyOp expression       #multiplyExpr
    | expression addOp expression            #addExpr
    | expression compareOp expression        #compareExpr
    | expression boolOp expression           #boolExpr // TODO:
    ;

variableDeclaration: 'var' IDENTIFIER '=' expression;
functionDeclaration: 'funky' IDENTIFIER '(' (parameterList)? ')' DATA_TYPES block;

parameterList: parameter (',' parameter)*;
parameter: DATA_TYPES IDENTIFIER;

constant: INTEGER | DOUBLE | STRING | BOOL | CHAR | NULL;
        
assignment: IDENTIFIER '=' expression;
functionCall: IDENTIFIER '(' (expression (',' expression)*)? ')';

statement: (variableDeclaration | assignment | functionCall | break | return) ';';

ifBlock: 'if' expression block ('else' elseIfBlock);
elseIfBlock: block | ifBlock;

whileBlock: WHILE expression block ('else' elseIfBlock);