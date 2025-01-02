﻿grammar Expressions;

// defines
WHILE: 'while';
INTEGER: [0-9]+;
DOUBLE: [0-9]+ '.' [0-9]+;
STRING: '"' (~["\r\n])* '"';
BOOL: 'true' | 'false';
CHAR: '\'' . '\'';
NULL: 'null';
WS: [ \t\r\n]+ -> skip;
DATA_TYPES: 'i16' | 'i32' | 'int' | 'i64' | 'string' | 'bool' | 'char' | 'double';
IDENTIFIER: [a-zA-Z_][a-zA-Z0-9_]*;

// grammar
program: line* EOF;
line: statement | ifBlock | whileBlock;
block: '{' line* '}';

multiplyOp: '*' | '/' | '%';
addOp: '+' | '-';
compareOp: '==' | '!=' | '>' | '<' | '>=' | '<=';
boolOp: 'and' | 'or' | 'xor';

break: 'break';

expression
    : constant                               #constantExpr
    | IDENTIFIER                             #identifierExpr
    | functionCall                           #functionCallExpr
    | '(' expression ')'                     #parenthesizedExpr
    | '!' expression                         #negationExpr
    | expression multiplyOp expression       #multiplyExpr
    | expression addOp expression            #addExpr
    | expression compareOp expression        #compareExpr
    | expression boolOp expression           #boolExpr
    ;

variableCreation: 'var' IDENTIFIER '=' expression;

constant: INTEGER | DOUBLE | STRING | BOOL | CHAR | NULL;
        
assignment: IDENTIFIER '=' expression;
functionCall: IDENTIFIER '(' (expression (',' expression)*)? ')';

statement: (variableCreation|assignment|functionCall|break) ';';

ifBlock: 'if' expression block ('else' elseIfBlock);
elseIfBlock: block | ifBlock;

whileBlock: WHILE expression block ('else' elseIfBlock);