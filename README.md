При построении парсера используется LL(1)-грамматика:
	S' -> ε | forS’’ | Doper’forS’’ | (Doper’)forS’’			;																																	
	S’’ -> ravS’’’ | ε																		;																									
	S’’’ -> ravS’’’’ | ε																	;																			
	S’’’’ -> rav | ε																			;																			
	rav -> =rav’																					;																		
	rav’ -> Goper																					;																															
	oper -> Doper’| (oper) | Foper’’ | <ID: булевское поле>| <const> | <reserved_word: substr>(M,M) | <ID: обычный>H’																		;
	oper’ -> HH | (oper)oper                             ;
	oper’’ -> Hoper* | (oper)oper* | Doper’ | Foper’’				;																																													
	oper* -> Hoper* | ε																																;																																
	H  -> <ID: булевское поле> | <const> | <reserved_word: substr>(M,M) | <ID: обычный>H’			;																											
	H’ -> [oper] | ε																														;																																		
	G -> <ID: булевское поле> | <ID: обычный>H’															;																																					
	M -> <ID: обычный>H’																									;																																					
	F  -> + | -																													;																																						
	D -> > | or | ^ 																									;																																							
																																																																									
По ней были построены множества FIRST и FOLLOW, после чего на основе полученных результатов построена таблица разбора M, реализованная в Parser.cs

