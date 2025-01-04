grammar Expressions;

// defines
WHILE: 'while';
FOR: 'for';
INTEGER: '-'? [0-9]+;
DOUBLE: '-'? [0-9]+ '.' [0-9]+;
STRING: '"' (~["\r\n])* '"';
BOOL: 'true' | 'false';
CHAR: '\'' . '\'';
NULL: 'null';
WS: [ \t\r\n]+ -> skip;
DATA_TYPES: 'i16' | 'i32' | 'i64' | 'string' | 'bool' | 'char' | 'double' | 'any' | 'void';
IDENTIFIER: [a-zA-Z_][a-zA-Z0-9_]*;
USE_IDENTIFIERS: 'disable-type-checking';

// grammar
program: line* EOF;
line:  packageDeclaration | usingDirective | classBlock | directive | statement | ifBlock | whileBlock | forBlock | functionDeclaration | return;
block: '{' line* '}'; 
directive: '#' 'use' IDENTIFIER ('-' IDENTIFIER)*;

multiplyOp: '*' | '/' | '%';
addOp: '+' | '-';
compareOp: '==' | 'is' | '!=' | 'is not' | '>' | '<' | '>=' | '<=';
boolOp: 'and' | 'or' | 'xor';
accessLevels: 'pub';

break: 'break';
continue: 'continue';
return: 'return' expression;

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
    | '(' DATA_TYPES ')' expression          #castExpr
    | expression '[' (expression) ']'        #arrayIndexExpr
    ;

packageDeclaration: 'package' IDENTIFIER;
usingDirective: 'using' IDENTIFIER;

variableDeclaration: 'var' IDENTIFIER '=' expression;
functionDeclaration: (accessLevels)? 'funky' IDENTIFIER '(' (parameterList)? ')' DATA_TYPES block;

parameterList: parameter (',' parameter)*;
parameter: IDENTIFIER DATA_TYPES;

constant: INTEGER | DOUBLE | STRING | BOOL | CHAR | NULL;

assignment: IDENTIFIER '=' expression;
operationAssignment: IDENTIFIER (addOp|multiplyOp) '=' expression;
selfOperation: ('++'|'--'|'**'|'//')? IDENTIFIER ('++'|'--'|'**'|'//')?;

functionCall
    : IDENTIFIER '(' (expression (',' expression)*)? ')'                    # simpleFunctionCall
    | IDENTIFIER '::' IDENTIFIER '(' (expression (',' expression)*)? ')'    # namespacedFunctionCall
    ;

statement: (variableDeclaration | assignment | operationAssignment | selfOperation | functionCall | break | continue | return) ';';

// Blocks
ifBlock: 'if' '('? expression ')'? block ('else' elseIfBlock)?;
elseIfBlock: block | ifBlock;

whileBlock: WHILE '('? expression ')'? block;
forBlock: FOR '('? (variableDeclaration | assignment)? ';' expression? ';' (selfOperation)? ')'? block;

classBlock: accessLevels? 'class' IDENTIFIER block; // TODO: future