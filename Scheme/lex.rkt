(module lex racket
	(require "csv.rkt")

	(provide analyze)

	(define analyze
		(lambda (file csv) (parse-csv csv))
	)
)
