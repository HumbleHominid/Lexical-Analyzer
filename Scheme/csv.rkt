(module csv racket
	(require racket/format)

	(provide parse-csv)

	; dfa format `(transition-function state->name)
	; *Note* Final states are defined as a state with no transitions
	; `(
	;		[(cur-state char) dest-state]
	;			transitions for state 0
	;		[(0 "a") 2] [(0 "b") 2] [(0 "\n") -1] [(0 "\r") 1]
	;			transitions for state 1
	;		[(1 "a") -1] [(1 "b") -1] [(1 "\n") 0]
	;			transitions for state 2
	;		[(2 "a") 2] [(2 "b") 2] [(2 "\n") -1]
	; )

	; Gets a csv file from the user and attempts to open it.
	; Recursively calls itself on file open failure
	; Makes the dfa if it can open the file
	(define parse-csv
		(lambda (path)
			(if (file-exists? path)
				(call-with-input-file path (lambda (in)
						(let* [(stream (open-input-string (read-line in `return-linefeed)))
					 			(dump1 (next-column stream))
								(dump2 (next-column stream))
								(headers (csv->list stream))]
							(make-transition-map in headers)
						)
					)
				)
				(display "Invalid path to file")
			)
		)
	)

	; Gets the next column in a csv string
	(define next-column
	  (lambda (in)
		  ; `let*` binds the vars synchronously
		  (let* [(raw-char (read-char in)) (pretty-char (~a raw-char))]
			 	(cond
					[(eof-object? raw-char) raw-char]
					[(string=? pretty-char ",") ""]
					[(non-empty-string? pretty-char)
						(let [(next (next-column in))]
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

	(define csv->list
		(lambda (in)
			(let [(next (next-column in))]
				(if (non-empty-string? next)
					(cons next (csv->list in))
					`()
				)
			)
		)
	)

	(define make-transition-map
		(lambda (in headers)
			(let [(next (read-line in `return-linefeed))]
				(if (eof-object? next)
					`()
				  (let* [(stream (open-input-string next))
							(state (next-column stream))
							(name (next-column stream))]
						(let [(row (make-row stream state 0 headers))]
							(if (empty? row)
								(make-transition-map in headers)
								(append row (make-transition-map in headers))
							)
						)
					)
				)
			)
		)
	)

	; Receives next-line as a port
	(define make-row
		(lambda (in state index headers)
			(let [(next-column (next-column in))]
			  (cond
					[(eof-object? next-column) `()]
				  [(string=? "" next-column) (make-row in state (+ index 1) headers)]
					[else (cons (list (list (string->number state) (list-ref headers
							index)) (string->number next-column)) (make-row in state
							(+ index 1) headers))]
				)
			)
		)
	)
)
