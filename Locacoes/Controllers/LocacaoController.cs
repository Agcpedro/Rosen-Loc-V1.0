﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Locacoes.Data;
using Locacoes.Models;
using Locacoes.Migrations;

namespace Locacoes.Controllers
{
    public class LocacaoController : Controller
    {
        private readonly LocacoesContext _context;

        public LocacaoController(LocacoesContext context)
        {
            _context = context;
        }

        // GET: Locacao
        public async Task<IActionResult> Index()
        {
            var locacoesContext = _context.Locacoes.Include(l => l.Cliente);
            return View(await locacoesContext.ToListAsync());
        }

        // GET: Locacao/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Incluindo os veículos locados junto com a locação
            var locacao = await _context.Locacoes
                .Include(l => l.Cliente)
                .Include(l => l.VeiculosLocados)
                    .ThenInclude(vl => vl.Veiculo) // Aqui você pode incluir a tabela Veiculo
                .FirstOrDefaultAsync(m => m.Id == id);

            if (locacao == null)
            {
                return NotFound();
            }

            return View(locacao);
        }

        // GET: Locacao/Create
        public IActionResult Create()
        {

            var veiculos = _context.Veiculo.Include(v => v.Modelo).ToList();

            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Nome");
            ViewData["VeiculoId"] = new SelectList(_context.Veiculo, "Id", "Modelo.Nome");

            ViewBag.VeiculoId = new SelectList(veiculos, "Id", "Modelo.Nome");
            return View();
        }

        // POST: Locacao/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DataLocacao,ValorTotal,ClienteId,VeiculoIds")] Locacao locacao)
        {
            if (locacao.VeiculosLocados == null)
            {
                locacao.VeiculosLocados = new List<VeiculoLocado>();
            }

            if (locacao.VeiculoIds == null || locacao.VeiculoIds.Count == 0)
            {
                ModelState.AddModelError("VeiculoIds", "Você deve selecionar pelo menos um veículo.");
            }

            if (ModelState.IsValid)
            {
                foreach (var veiculoId in locacao.VeiculoIds)
                {
                    locacao.VeiculosLocados.Add(new VeiculoLocado { VeiculoId = veiculoId });
                }

                _context.Add(locacao);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Nome", locacao.ClienteId);
            ViewData["VeiculoId"] = new SelectList(_context.Veiculo, "Id", "Modelo.Nome");
            return View(locacao);
        }

        // GET: Locacao/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var locacao = await _context.Locacoes.FindAsync(id);
            if (locacao == null)
            {
                return NotFound();
            }
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Id", locacao.ClienteId);
            return View(locacao);
        }

        // POST: Locacao/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DataLocacao,ValorTotal,ClienteId")] Locacao locacao)
        {
            if (id != locacao.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(locacao);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LocacaoExists(locacao.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Id", locacao.ClienteId);
            return View(locacao);
        }

        // GET: Locacao/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var locacao = await _context.Locacoes
                .Include(l => l.Cliente)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (locacao == null)
            {
                return NotFound();
            }

            return View(locacao);
        }

        // POST: Locacao/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var locacao = await _context.Locacoes.FindAsync(id);
            if (locacao != null)
            {
                _context.Locacoes.Remove(locacao);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LocacaoExists(int id)
        {
            return _context.Locacoes.Any(e => e.Id == id);
        }
    }
}
