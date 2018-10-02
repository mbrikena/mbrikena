﻿using System;
using System.Collections.Generic;
using System.Linq;
using LibraryManagement.Data.Intefaces;
using LibraryManagement.Data.Model;
using LibraryManagement.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Controllers
{
	public class BookController : Controller
	{
		private readonly IBookRepository _bookRepository;

		private readonly IAuthorRepository _authorRepository;

		public BookController(IBookRepository bookRepository, IAuthorRepository authorRepository)
		{
			_bookRepository = bookRepository;
			_authorRepository = authorRepository;
		}

		[Route("Book")]
		public IActionResult List(int? authorId, int? borrowerId)
		{
			if (authorId == null && borrowerId == null)
			{
				//show all books
				var books = _bookRepository.GetAllWithAuthor();
				//check books
				return CheckBooks(books);
			}
			else if(authorId != null)
			{
				//filter by authorId
				var author = _authorRepository.GetWithBooks((int)authorId);
				//check author books
				if (author.Books.Count() == 0)
				{
					return View("AuthorEmpty", author);
				}
				else
				{
					return View(author.Books);
				}
			}
			else if(borrowerId != null)
			{
				//filter ny borrowerId
				var books = _bookRepository
					.FindWithAuthorAndBorrower(b => b.BorrowerId == borrowerId);
				//check borrower books
				return CheckBooks(books);
			}
			else
			{
				//throw exeption
				throw new ArgumentException();
			}
		}

		public IActionResult CheckBooks(IEnumerable<Book>books)
		{
			if (books.Count() == 0)
			{
				return View("Empty");
			}
			else
			{
				return View(books);
			}
		}

		public IActionResult Create()
		{
			var bookVM = new BookViewModel()
			{
				Authors = _authorRepository.GetAll()
			};
			return View(bookVM);
		}

		[HttpPost]
		public IActionResult Create(BookViewModel bookViewModel)
		{
			if (!ModelState.IsValid)
			{
				bookViewModel.Authors = _authorRepository.GetAll();
				return View(bookViewModel);
			}

			_bookRepository.Create(bookViewModel.Book);

			return RedirectToAction("List");
		}

		public IActionResult Update(int id)
		{
			var bookVM = new BookViewModel()
			{
				Book = _bookRepository.GetById(id),
				Authors = _authorRepository.GetAll()
			};

			return View(bookVM);
		}

		[HttpPost]
		public IActionResult Update(BookViewModel bookViewModel)
		{
			_bookRepository.Update(bookViewModel.Book);

			return RedirectToAction("List");
		}

		public IActionResult Delete(int id)
		{
			var book = _bookRepository.GetById(id);

			_bookRepository.Delete(book);

			return RedirectToAction("List");
		}
	}
}