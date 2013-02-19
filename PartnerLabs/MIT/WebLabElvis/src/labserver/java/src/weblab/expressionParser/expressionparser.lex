package weblab.expressionParser;

import java_cup.runtime.Symbol;


%%


// make generated Yylex class a public class
%public

// options for compatibility with Java CUP
%implements java_cup.runtime.Scanner
%function next_token
%type java_cup.runtime.Symbol

// return the EOF token when end of file is reached
%eofval{
  return new Symbol(Sym.EOF);
%eofval}

// indicate that lexical actions might throw InvalidExpressionException
%yylexthrow{
InvalidExpressionSyntaxException
%yylexthrow}

// bind variable yychar to zero-based character index of matched text
%char



/* some useful regexp macros */

ALPHA=[A-Za-z]
DIGIT=[0-9]
// TODO: do we want newline chars here or not?? <dmrz>
WHITESPACE_CHAR=[" "\t\n\f\r\b]

// data variable names (max 6 chars, but can't specify that in regexp)
// TODO: I'm not sure about the underscore; the manual isn't clear. <dmrz>
DATA_VARIABLE_NAME = ({ALPHA}({ALPHA}|{DIGIT}|_)*)

// numeric constants recognized by the HP4155
// Note: yes, "." by itself is a valid number (it evaluates to zero).
MANTISSA          = (({DIGIT}+)|({DIGIT}*\.{DIGIT}*))
EXPONENT_NOTATION = (([eE][-\+]?{DIGIT}+)|([fpnumkMG]))
NUMERIC_CONSTANT  = ({MANTISSA}{EXPONENT_NOTATION}?)

// scientific constants recognized by the HP4155
SCIENTIFIC_CONSTANT = [qke]

// built-in function keywords
BUILTIN_FUNCTION_KEYWORD = (ABS|AT|AVG|COND|DELTA|DIFF|EXP|INTEG|LGT|LOG|MAVG|MAX|MIN|SQRT)

// Note: the lexer does not currently recognize the HP4155's read out
// function keywords.


%% 


{WHITESPACE_CHAR}+ { /* ignore whitespace */ }

"," { return new Symbol(Sym.COMMA,
			yychar, yychar+yytext().length(), yytext()); }
"(" { return new Symbol(Sym.LPAREN,
			yychar, yychar+yytext().length(), yytext()); }
")" { return new Symbol(Sym.RPAREN,
			yychar, yychar+yytext().length(), yytext()); }
"+" { return new Symbol(Sym.PLUS,
			yychar, yychar+yytext().length(), yytext()); }
"-" { return new Symbol(Sym.MINUS,
			yychar, yychar+yytext().length(), yytext()); }
"*" { return new Symbol(Sym.TIMES,
			yychar, yychar+yytext().length(), yytext()); }
"/" { return new Symbol(Sym.DIVIDE,
			yychar, yychar+yytext().length(), yytext()); }
"^" { return new Symbol(Sym.POW,
			yychar, yychar+yytext().length(), yytext()); }

{NUMERIC_CONSTANT} {
  return new Symbol(Sym.NUMBER, yychar, yychar+yytext().length(), yytext());
}

{SCIENTIFIC_CONSTANT} {
  return new Symbol(Sym.SCIENTIFIC_CONST, yychar, yychar+yytext().length(),
		    yytext());
}

{BUILTIN_FUNCTION_KEYWORD} {
  return new Symbol(Sym.BUILTIN_FUNCTION, yychar, yychar+yytext().length(),
		    yytext());
}

{DATA_VARIABLE_NAME} {
  return new Symbol(Sym.VARIABLE, yychar, yychar+yytext().length(), yytext());
}

. {
  // unexpected input; throw an exception
  String m = "unexpected character \'" + yytext() + "\' at character " +
    yychar + " of input";
  throw new InvalidExpressionSyntaxException(m);
}
