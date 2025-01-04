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
DATA_TYPES: 'i16' | 'i32' | 'i64' | 'string' | 'bool' | 'char' | 'double' | 'any' | 'void';
IDENTIFIER: [a-zA-Z_][a-zA-Z0-9_]*;
USE_IDENTIFIERS: 'disable-type-checking';

// grammar
program: line* EOF;
line:  packageDeclaration | usingDirective | classBlock | directive | statement | ifBlock | whileBlock | forBlock | foreachBlock | functionDeclaration | return;
block: '{' line* '}'; 
directive: '#' 'use' USE_IDENTIFIERS;

multiplyOp: '*' | '/' | '%';
addOp: '+' | '-';
compareOp: '==' | 'is' | '!=' | 'is not' | '>' | '<' | '>=' | '<=';
boolOp: 'and' | 'or' | 'xor';
accessLevels: 'pub';

arrayLiteral: '[' (expression (',' expression)*)? ']' ;
arrayAccessor: '[' (expression) ']';
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
    | expression multiplyOp expression       #multiplyExpr
    | expression addOp expression            #addExpr
    | expression compareOp expression        #compareExpr
    | expression boolOp expression           #boolExpr
    | '(' DATA_TYPES ')' expression          #castExpr
    | arrayLength                            #arrayLengthExpr
    ;

packageDeclaration: 'package' IDENTIFIER;
usingDirective: 'using' IDENTIFIER;

variableDeclaration: 'var' IDENTIFIER '=' expression;
functionDeclaration: (accessLevels)? 'funky' IDENTIFIER '(' (parameterList)? ')' DATA_TYPES block;
arrayDeclaration: 'var' IDENTIFIER '=' arrayLiteral DATA_TYPES;

parameterList: parameter (',' parameter)*;
parameter: IDENTIFIER DATA_TYPES;

constant: INTEGER | DOUBLE | STRING | BOOL | CHAR | NULL;

assignment: IDENTIFIER arrayAccessor? '=' expression;
operationAssignment: IDENTIFIER (addOp|multiplyOp) '=' expression;
selfOperation: ('++'|'--'|'**'|'//')? IDENTIFIER ('++'|'--'|'**'|'//')?;

functionCall
    : IDENTIFIER '(' (expression (',' expression)*)? ')'                    # simpleFunctionCall
    | IDENTIFIER '::' IDENTIFIER '(' (expression (',' expression)*)? ')'    # namespacedFunctionCall
    ;

statement: (variableDeclaration | arrayDeclaration | assignment | operationAssignment | selfOperation | functionCall | break | continue | return) ';';

// Blocks
ifBlock: 'if' '('? expression ')'? block ('else' elseIfBlock)?;
elseIfBlock: block | ifBlock;

whileBlock: WHILE '('? expression ')'? block;
forBlock: FOR '('? (variableDeclaration | assignment)? ';' expression? ';' (selfOperation)? ')'? block;
foreachBlock: FOREACH '(' 'var' IDENTIFIER 'in' expression ')' block;

classBlock: accessLevels? 'class' IDENTIFIER block; // TODO: future