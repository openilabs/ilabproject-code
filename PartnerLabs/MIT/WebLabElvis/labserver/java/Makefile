#

# Makefile for Weblab java module which validates user-defined functions.

.SUFFIXES:
.SUFFIXES: .java .class

srcdir = src
classdir = bin

JAVAC = javac
JOPTS =  -d $(classdir) -sourcepath $(srcdir)

JAVA = java
JAVAOPTS = -classpath $(classdir)

# the packages to build
packages = \
weblab.expressionParser \
java_cup.runtime \
weblab.validation \

# contents of named package (filename stems only)
weblab.validation = \
Validation \

# contents of named package (filename stems only)
java_cup.runtime = \
lr_parser \
Scanner \
Symbol \
virtual_parse_stack \

# contents of named package (filename stems only)
weblab.expressionParser = \
Yylex \
Parser \
Sym \
BinaryOpExpression \
BuiltinFunctionExpression \
Expression \
ExpressionList \
ExpressionVisitor \
ExpressionVisitorException \
InvalidExpressionSyntaxException \
NumericConstantExpression \
ScientificConstantExpression \
SemanticCheckingVisitor \
UnaryOpExpression \
VariableExpression \


# auto-generated list of all .class files in all relevant packages
classfiles = $(foreach pkg,$(packages),\
$(foreach file,$($(pkg)),$(classdir)/$(subst .,/,$(pkg))/$(file).class))


.PHONY: all build echo clean JLex Java_CUP doc

all: build

# build all classfiles
build: outdirs $(srcdir)/weblab/expressionParser/Yylex.java $(srcdir)/weblab/expressionParser/Parser.java $(classfiles)

echo:
	@echo $(classfiles)


# generate Yylex.java using JLex
$(srcdir)/weblab/expressionParser/Yylex.java: \
$(srcdir)/weblab/expressionParser/expressionparser.lex \
$(classdir)/JLex/Main.class
	$(JAVA) $(JAVAOPTS) JLex.Main $<
	mv $<.java $@


# generate Parser.java and Sym.java using Java_CUP
$(srcdir)/weblab/expressionParser/Parser.java \
$(srcdir)/weblab/expressionParser/Sym.java: \
$(srcdir)/weblab/expressionParser/expressionparser.cup \
$(classdir)/java_cup/Main.class
	$(JAVA) $(JAVAOPTS) java_cup.Main -parser Parser -symbols Sym < $<
	mv Parser.java $(srcdir)/weblab/expressionParser/
	mv Sym.java $(srcdir)/weblab/expressionParser/


# build JLex
JLex: $(classdir)/JLex/Main.class


# build Java_CUP
Java_CUP: $(classdir)/java_cup/Main.class 


# compile any .class file from corresponding .java file
$(classdir)/%.class: $(srcdir)/%.java
	$(JAVAC) $(JOPTS) $<


clean:
	rm -r $(classdir)

outdirs:
	if ! [ -d $(classdir) ]; then mkdir $(classdir); fi
