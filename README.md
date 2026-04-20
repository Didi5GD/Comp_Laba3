Лабораторная работа 3. Разработка синтаксического анализатора (парсера)
Автор: Горащенко Дарья Романовна, факультет АВТФ,курс 3, семестр 6, группа АВТ-313.

Цели: сделать лексер, с алгоритм нейтрализации синтаксических ошибок методом Айронса по своему варианту (if-else PHP). Вывод ошибок в табличку. 

-Реализовать навигацию по ошибкам: при щелчке на сообщении об ошибке в таблице результатов курсор в области редактирования должен устанавливаться на позицию недопустимого символа.

Вариант задания: 94. Условный оператор if-else на языке PHP

Примеры кода корректные (многострочные в том числе) 
 if ($a > $b) { $max = $a; } else { $max = $b; };


if($a>$b){ $a--;} else{ $a=$b; };

граматика: 
1.   <Start> -> 'if' <CONDITION> <BLOCK> <ELSE_PART> <END_SEMI>
2.   <CONDITION> -> '(' <EXPR> ')'
3.   <EXPR> -> <LOGIC_TERM> { '||' <LOGIC_TERM> }
4.   <LOGIC_TERM> -> <COMPARE> { '&&' <COMPARE> }
5.   <COMPARE> -> <VALUE> <REL_OP> <VALUE> | '(' <EXPR> ')'
6.   <VALUE> -> '$' <ID> 
7.   <REL_OP> -> '>' | '<' | '>=' | '<=' | '==' | '!='
8.   <ID> -> letter <ID_TAIL>
9.   <ID_TAIL> -> letter <ID_TAIL> | digit <ID_TAIL> | '_' <ID_TAIL> | ε
10.  <BLOCK> -> '{' <STATEMENT_LIST> '}'
11.  <STATEMENT_LIST> -> <STATEMENT> { <STATEMENT> }
12.  <STATEMENT> -> <VALUE> <OP> <VAR> ';'
13.  <OP>-> ‘=’|’++’|’--’
14.  <VAR> -> '$' <ID> | ε
15.  <ELSE_PART> -> 'else' <ELSE_BLOCK> | ε
16.  <ELSE_BLOCK> -> '{' <STATEMENT_LIST> '}'
17.  <END_SEMI> -> ';'

V_t = { 'if', 'else', '(', ')', '{', '}', ';', '$', '=', '>', '<', '>=', '<=', '==', '!=', '||', ’++’,’--' '&&', letter, digit, '_' }
V_N = { <Start>, <CONDITION>, <EXPR>, <LOGIC_TERM>, <COMPARE>, <VALUE>, <LOGIC_OP>, <AND_OP>, <REL_OP>, <ID>, <ID_TAIL>, <BLOCK>, <STATEMENT_LIST>, <STATEMENT>, <VAR>, <ELSE_PART>, <ELSE_BLOCK>, <END_SEMI>, <OP> }


Перечень допустимых лексем: идентификатор (начинается с $ а дальше любые буквы и цифры), оператор if, оператор else, оператор elseif, оператор сложения, оператор инкремент, оператор вычитания, оператор декремент, фигурная скобка закрывающая, фигурная скобка открывающая, оператор присваивания(=), оператор равенства(==), оператор деления, круглая скобка закрывающая, круглая скобка открывающая ,оператор неравенства (!=), оператор остаток от деления, оператор меньше, оператор меньше или равно, оператор больше, оператор больше или равно, конец оператор (;), оператор умножения(*), оператор возведения в степень (**), оператор инкримент, оператор дикримент если ничего из этого то ERROR.

<img width="1063" height="818" alt="image" src="https://github.com/user-attachments/assets/70191a80-c131-4e34-9378-9c8c3300345b" />

скрирны  
<img width="955" height="584" alt="image" src="https://github.com/user-attachments/assets/a05a6120-95fb-4198-9ee9-54984027edab" />
<img width="899" height="575" alt="image" src="https://github.com/user-attachments/assets/2e726a1d-5b25-487e-b012-8625d5af1011" />

<img width="1161" height="664" alt="image" src="https://github.com/user-attachments/assets/7618d17a-9805-45e0-acee-87a39a2d2274" />
<img width="897" height="577" alt="image" src="https://github.com/user-attachments/assets/8220526b-f85c-471a-aa57-f19cbbbbad12" />


