(module csv racket
	(require racket/format)

	(provide parse-csv)
	
	; dfa format `( current-state transition-function )
	; *Note* Final states are defined as a state with no transitions
	; `(0 
	;	(
	;		[(cur-state char) dest-state]
	;			transitions for state 0
	;		[(0 "a") 2] [(0 "b") 2] [(0 "\n") -1] [(0 "\r") 1]
	;			transitions for state 1
	;		[(1 "a") -1] [(1 "b") -1] [(1 "\n") 0]
	;			transitions for state 2
	;		[(2 "a") 2] [(2 "b") 2] [(2 "\n") -1]
	;	)
	; )

	; Gets a csv file from the user and attempts to open it.
	; Recursively calls itself on file open failure
	; Makes the dfa if it can open the file
	(define parse-csv
		(lambda ()
			(let [(path (_get-source-file))]
				(if (file-exists? path)
					(call-with-input-file path (lambda (in) (_make-dfa in)))		
					(begin
						(display "Invalid path to file")
						(parse-csv)
					)
				)
			)
		)
	)

	; Gets the next column in a csv string
	(define _next-column
	  	(lambda (in)
		  	; `let*` binds the vars syncronously
		  	(let* [(raw-char (read-char in)) (pretty-char (~a raw-char))]
			  	(cond 
					[(eof-object? raw-char) raw-char]
					[(string=? pretty-char ",") ""]
					[(non-empty-string? pretty-char)
							(let [(next (_next-column in))]
								(if (eof-object? next)
									pretty-char
									(string-append pretty-char next)
								)
							)
					]
					[else ""]
				)
			)
		)
	)

	(define _make-dfa
		(lambda (in)
			; Make indexed list of chars
			(let* [(stream (open-input-string (read-line in `return-linefeed)))
			 		(dump1 (_next-column stream))
					(dump2 (_next-column stream))
					(headers (_csv-to-list stream))]
				(cons 0 (list (_make-transition-map in headers)))
			)
		)
	)

	(define _csv-to-list
		(lambda (in)
			(let [(next (_next-column in))]
				(if (non-empty-string? next)
					(cons next (_csv-to-list in))
					`()
				)
			)
		)
	)

	(define _make-transition-map
		(lambda (in headers)
			(let [(next (read-line in `return-linefeed))]
				(if (eof-object? next)
					`()
				  	(let* [(stream (open-input-string next))
							(state (_next-column stream))
							(name (_next-column stream))]
						(let [(row (_make-row stream state 0 headers))]
							(if (empty? row)
								(_make-transition-map in headers)
								(cons row (_make-transition-map in headers))
							)
						)
					)
				)
			)
		)
	)

	; Recieves next-line as a port
	(define _make-row
		(lambda (in state index headers)
			(let [(next-column (_next-column in))]
			  (cond
					[(eof-object? next-column) `()]
				  	[(string=? "" next-column) (_make-row in state (+ index 1) headers)]
					[else (cons (list (list (string->number state) (list-ref headers index))
							(string->number next-column))
							(_make-row in state (+ index 1) headers))]
				)
			)
		)
	)

	(define _get-source-file 
		(lambda ()
	  		(begin
				(display "Give csv file> ")
				(~a (read))
			)
		)
	)
)
