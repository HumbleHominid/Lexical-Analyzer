(module lex racket
	(require "csv.rkt")

	(provide analyze)

  (define stateNames
    `(
      (3 "Comma")
      (4 "Left Brace")
      (5 "Logical Binary Op")
      (6 "Left Paren")
      (7 "Right Brace")
      (8 "Right Paren")
      (9 "Semicolon")
      (10 "Multiplication Op")
      (15 "Line Comment")
      (16 "Block Comment")
      (17  "Identifier")
      (18  "Bool Constant")
      (19  "Integer Constant")
      (20  "Integer Constant")
      (21  "Addition Op")
      (22  "Addition Op")
      (23  "Multiplication Op")
      (24  "Assignment Op")
      (26  "Increment Op")
      (27  "Float Constant")
      (28  "Comparator")
      (29  "Logical Not")
      (30  "Comparator")
      (31  "Assignment Op")
    )
  )

  (define keywords
    `(
      ("bool" "Boolean Keyword")
      ("int" "Integer Keyword")
      ("while" "While Keyword")
      ("if" "If Keyword")
      ("else" "Else Keyword")
      ("return" "Return Keyword")
      ("float" "Float Keyword")
      ("main" "Main Keyword")
    )
  )

	(define analyze
		(lambda (file csv)
      (if (file-exists? file)
        (call-with-input-file file (lambda (program) (parse-file program
            (parse-csv csv)  0 ""))
        )
        (display "Invalid path to file")
      )
    )
	)

  (define parse-file
    (lambda (program dfa state token)
      (let* [(nextChar (~a (read-char program)))
          (nextState (assoc (list state nextChar) dfa))]
        (display (list nextChar nextState (list state nextChar) dfa))
        (cond
          [(list? nextState)
            (if (= (cadr nextState) 0)
              (parse-file program dfa (cadr nextState) "")
              (parse-file program dfa (cadr nextState) (string-append nextChar token))
            )]
          [(eof-object? nextChar) (display "End of file")]
          [else (let* [(name (assoc state stateNames)) (newName (assoc name keywords))]
            (if (list? newName)
              (display (list "Token: " token ", Name: " newName))
              (display (list "Token: " token ", Name: " name))
            )
            (display "There was an error.")
          )]
        )
      )
    )
  )
)
