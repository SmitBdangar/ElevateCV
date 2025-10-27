"""
AI-Powered Feedback Generator and Resume Rewriter
Uses OpenAI GPT for intelligent feedback and content improvement
"""

from typing import Dict, List, Optional
import openai
from dataclasses import dataclass
import json

@dataclass
class FeedbackReport:
    """Structured feedback report"""
    strengths: List[str]
    weaknesses: List[str]
    missing_skills: List[str]
    actionable_tips: List[str]
    rewritten_sections: Dict[str, str]
    overall_assessment: str
    priority_actions: List[str]

class AIFeedbackGenerator:
    """Generate intelligent feedback using LLMs"""
    
    def __init__(self, api_key: str, model: str = "gpt-3.5-turbo"):
        """
        Initialize with OpenAI API key
        
        Args:
            api_key: OpenAI API key
            model: Model to use (gpt-3.5-turbo, gpt-4, etc.)
        """
        self.api_key = api_key
        self.model = model
        openai.api_key = api_key
    
    def generate_comprehensive_feedback(
        self,
        resume_data: Dict,
        job_data: Dict,
        score_details: Dict
    ) -> FeedbackReport:
        """Generate detailed, actionable feedback"""
        
        prompt = self._build_feedback_prompt(resume_data, job_data, score_details)
        
        try:
            response = openai.chat.completions.create(
                model=self.model,
                messages=[
                    {
                        "role": "system",
                        "content": """You are an expert resume coach and hiring manager with 15+ years of experience. 
                        You provide actionable, specific feedback that helps candidates improve their resumes.
                        Focus on concrete improvements, not generic advice."""
                    },
                    {
                        "role": "user",
                        "content": prompt
                    }
                ],
                temperature=0.7,
                max_tokens=1500
            )
            
            feedback_text = response.choices[0].message.content
            
            # Parse structured feedback
            return self._parse_feedback_response(feedback_text, score_details)
        
        except Exception as e:
            print(f"Error generating feedback: {e}")
            return self._generate_fallback_feedback(resume_data, job_data, score_details)
    
    def _build_feedback_prompt(
        self,
        resume_data: Dict,
        job_data: Dict,
        score_details: Dict
    ) -> str:
        """Build detailed prompt for feedback generation"""
        
        prompt = f"""
Analyze this resume against the job requirements and provide specific, actionable feedback.

# JOB REQUIREMENTS
Required Skills: {', '.join(job_data.get('required_skills', []))}
Preferred Skills: {', '.join(job_data.get('preferred_skills', []))}
Experience Level: {job_data.get('required_level', 'Not specified')}
Years Required: {job_data.get('required_years', 'Not specified')}

# CANDIDATE'S RESUME
Skills: {', '.join(resume_data.get('skills', []))}
Experience: {len(resume_data.get('experience', []))} positions
Education: {len(resume_data.get('education', []))} degrees

# ASSESSMENT SCORES
Overall Match: {score_details.get('overall_score', 0):.1f}/100
Skill Match: {score_details.get('skill_match_score', 0):.1f}/100
Experience Match: {score_details.get('experience_match_score', 0):.1f}/100

Missing Critical Skills: {', '.join(score_details.get('missing_critical_skills', [])[:5])}

# REQUIRED OUTPUT
Provide feedback in this exact format:

## STRENGTHS (Top 3)
1. [Specific strength with evidence]
2. [Specific strength with evidence]
3. [Specific strength with evidence]

## WEAKNESSES (Top 3)
1. [Specific weakness with impact]
2. [Specific weakness with impact]
3. [Specific weakness with impact]

## CRITICAL MISSING SKILLS
- [Skill 1]: Why it matters and how to demonstrate it
- [Skill 2]: Why it matters and how to demonstrate it
- [Skill 3]: Why it matters and how to demonstrate it

## ACTIONABLE IMPROVEMENTS (Priority Order)
1. [Specific action with example]
2. [Specific action with example]
3. [Specific action with example]
4. [Specific action with example]
5. [Specific action with example]

## OVERALL ASSESSMENT
[2-3 sentence summary of candidacy and main recommendation]

Be specific, actionable, and honest. Avoid generic advice.
"""
        return prompt
    
    def rewrite_resume_section(
        self,
        section_text: str,
        section_type: str,
        job_requirements: List[str],
        improvement_focus: str = "impact"
    ) -> str:
        """
        Rewrite a resume section for better impact
        
        Args:
            section_text: Original text to rewrite
            section_type: Type (experience, summary, skills)
            job_requirements: List of job requirements to align with
            improvement_focus: What to improve (impact, keywords, clarity)
        """
        
        prompt = f"""
Rewrite this resume {section_type} to be more compelling and aligned with job requirements.

# ORIGINAL TEXT
{section_text}

# JOB REQUIREMENTS TO EMPHASIZE
{', '.join(job_requirements[:5])}

# IMPROVEMENT FOCUS
{improvement_focus}

# GUIDELINES
- Use strong action verbs (achieved, led, implemented, optimized)
- Quantify results with metrics when possible (%, $, time saved)
- Highlight relevant skills naturally
- Keep it concise (no more than 20% longer than original)
- Maintain truthfulness - only rephrase, don't invent claims

# OUTPUT
Provide the rewritten section only, no explanations.
"""
        
        try:
            response = openai.chat.completions.create(
                model=self.model,
                messages=[
                    {
                        "role": "system",
                        "content": "You are an expert resume writer who creates ATS-friendly, impactful content."
                    },
                    {
                        "role": "user",
                        "content": prompt
                    }
                ],
                temperature=0.8,
                max_tokens=500
            )
            
            return response.choices[0].message.content.strip()
        
        except Exception as e:
            print(f"Error rewriting section: {e}")
            return section_text
    
    def suggest_bullet_points(
        self,
        job_title: str,
        company: str,
        job_requirements: List[str],
        num_bullets: int = 3
    ) -> List[str]:
        """Generate strong bullet points for experience section"""
        
        prompt = f"""
Create {num_bullets} impactful resume bullet points for this role that align with target job requirements.

# ROLE DETAILS
Position: {job_title}
Company: {company}

# TARGET JOB REQUIREMENTS
{', '.join(job_requirements)}

# GUIDELINES
- Start with strong action verbs
- Include metrics/results (use realistic examples like "improved X by Y%")
- Highlight transferable skills
- Keep each bullet to 1-2 lines
- Make them ATS-friendly

# OUTPUT FORMAT
Return only the bullet points, one per line, starting with "• "
"""
        
        try:
            response = openai.chat.completions.create(
                model=self.model,
                messages=[
                    {
                        "role": "system",
                        "content": "You are a professional resume writer specializing in achievement-focused bullet points."
                    },
                    {
                        "role": "user",
                        "content": prompt
                    }
                ],
                temperature=0.8,
                max_tokens=300
            )
            
            bullets_text = response.choices[0].message.content
            # Parse bullets
            bullets = [line.strip() for line in bullets_text.split('\n') if line.strip().startswith('•')]
            return bullets[:num_bullets]
        
        except Exception as e:
            print(f"Error generating bullets: {e}")
            return [
                f"• Managed key projects for {company} focusing on relevant technologies",
                f"• Collaborated with cross-functional teams to deliver results",
                f"• Improved processes and systems efficiency"
            ][:num_bullets]
    
    def optimize_for_ats(
        self,
        resume_text: str,
        target_keywords: List[str]
    ) -> Dict[str, any]:
        """
        Analyze and optimize resume for ATS systems
        
        Returns analysis of keyword usage and suggestions
        """
        
        resume_lower = resume_text.lower()
        
        # Check keyword presence
        keyword_analysis = {}
        for keyword in target_keywords:
            count = resume_lower.count(keyword.lower())
            keyword_analysis[keyword] = {
                'present': count > 0,
                'frequency': count,
                'status': 'good' if count >= 2 else ('present' if count == 1 else 'missing')
            }
        
        # Calculate ATS score
        present_keywords = sum(1 for k in keyword_analysis.values() if k['present'])
        ats_score = (present_keywords / len(target_keywords) * 100) if target_keywords else 0
        
        # Generate suggestions
        missing_keywords = [k for k, v in keyword_analysis.items() if not v['present']]
        underused_keywords = [k for k, v in keyword_analysis.items() if v['frequency'] == 1]
        
        suggestions = []
        if missing_keywords:
            suggestions.append(f"Add these critical keywords: {', '.join(missing_keywords[:5])}")
        if underused_keywords:
            suggestions.append(f"Use these keywords more frequently: {', '.join(underused_keywords[:3])}")
        if ats_score < 70:
            suggestions.append("Increase keyword density by naturally incorporating job terms in your experience descriptions")
        
        return {
            'ats_score': round(ats_score, 1),
            'keyword_analysis': keyword_analysis,
            'missing_keywords': missing_keywords,
            'suggestions': suggestions
        }
    
    def _parse_feedback_response(
        self,
        feedback_text: str,
        score_details: Dict
    ) -> FeedbackReport:
        """Parse LLM response into structured feedback"""
        
        # Simple parsing (can be enhanced with better structure)
        lines = feedback_text.split('\n')
        
        strengths = []
        weaknesses = []
        tips = []
        current_section = None
        
        for line in lines:
            line = line.strip()
            if 'STRENGTH' in line.upper():
                current_section = 'strengths'
            elif 'WEAKNESS' in line.upper():
                current_section = 'weaknesses'
            elif 'IMPROVEMENT' in line.upper() or 'ACTION' in line.upper():
                current_section = 'tips'
            elif line and (line[0].isdigit() or line.startswith('-') or line.startswith('•')):
                clean_line = line.lstrip('0123456789.-• ')
                if current_section == 'strengths' and len(strengths) < 3:
                    strengths.append(clean_line)
                elif current_section == 'weaknesses' and len(weaknesses) < 3:
                    weaknesses.append(clean_line)
                elif current_section == 'tips' and len(tips) < 5:
                    tips.append(clean_line)
        
        return FeedbackReport(
            strengths=strengths or ["Shows relevant skills", "Has work experience", "Education background"],
            weaknesses=weaknesses or ["Could improve keyword usage", "May need more quantified achievements", "Consider adding certifications"],
            missing_skills=score_details.get('missing_critical_skills', [])[:5],
            actionable_tips=tips or ["Tailor resume to job description", "Add metrics to achievements", "Use stronger action verbs"],
            rewritten_sections={},
            overall_assessment=self._extract_assessment(feedback_text),
            priority_actions=tips[:3] if tips else ["Review job requirements", "Update skills section", "Quantify achievements"]
        )
    
    def _extract_assessment(self, text: str) -> str:
        """Extract overall assessment from feedback"""
        lines = text.split('\n')
        for i, line in enumerate(lines):
            if 'OVERALL' in line.upper() or 'ASSESSMENT' in line.upper():
                # Get next few non-empty lines
                assessment_lines = []
                for j in range(i+1, min(i+5, len(lines))):
                    if lines[j].strip():
                        assessment_lines.append(lines[j].strip())
                return ' '.join(assessment_lines)
        return "Review the detailed feedback for specific improvement areas."
    
    def _generate_fallback_feedback(
        self,
        resume_data: Dict,
        job_data: Dict,
        score_details: Dict
    ) -> FeedbackReport:
        """Generate rule-based feedback as fallback"""
        
        score = score_details.get('overall_score', 0)
        matched_skills = score_details.get('matched_skills', [])
        missing_skills = score_details.get('missing_critical_skills', [])
        
        # Generate strengths
        strengths = []
        if len(matched_skills) >= 3:
            strengths.append(f"Strong technical skill alignment with {len(matched_skills)} matching skills")
        if len(resume_data.get('experience', [])) >= 2:
            strengths.append(f"Solid work history with {len(resume_data.get('experience', []))} relevant positions")
        if resume_data.get('education'):
            strengths.append("Educational qualifications meet requirements")
        
        # Generate weaknesses
        weaknesses = []
        if missing_skills:
            weaknesses.append(f"Missing {len(missing_skills)} critical skills: {', '.join(missing_skills[:3])}")
        if score < 70:
            weaknesses.append("Resume lacks sufficient keyword optimization for ATS systems")
        if len(resume_data.get('experience', [])) < 2:
            weaknesses.append("Limited work experience documentation")
        
        # Generate actionable tips
        tips = []
        if missing_skills:
            tips.append(f"Immediately acquire or highlight these skills: {', '.join(missing_skills[:2])}")
        tips.append("Rewrite experience bullets using STAR method (Situation, Task, Action, Result)")
        tips.append("Add quantifiable metrics to at least 50% of your achievements")
        tips.append("Include relevant projects or portfolio links demonstrating key skills")
        tips.append("Optimize resume with keywords from job description")
        
        # Overall assessment
        if score >= 80:
            assessment = "Strong candidate with excellent alignment. Minor refinements recommended."
        elif score >= 65:
            assessment = "Good potential fit. Focus on highlighting missing skills and quantifying achievements."
        elif score >= 50:
            assessment = "Moderate fit. Significant improvements needed in skill alignment and presentation."
        else:
            assessment = "Weak alignment with requirements. Consider gaining experience in critical missing skills."
        
        return FeedbackReport(
            strengths=strengths[:3],
            weaknesses=weaknesses[:3],
            missing_skills=missing_skills[:5],
            actionable_tips=tips,
            rewritten_sections={},
            overall_assessment=assessment,
            priority_actions=tips[:3]
        )
    
    def generate_cover_letter(
        self,
        resume_data: Dict,
        job_data: Dict,
        company_name: str,
        hiring_manager: str = "Hiring Manager"
    ) -> str:
        """Generate a tailored cover letter"""
        
        prompt = f"""
Write a professional cover letter for this candidate applying to {company_name}.

# CANDIDATE BACKGROUND
Skills: {', '.join(resume_data.get('skills', [])[:8])}
Recent Experience: {resume_data.get('experience', [{}])[0].get('title', 'Professional') if resume_data.get('experience') else 'Experienced professional'}
Education: {resume_data.get('education', [{}])[0].get('degree', '') if resume_data.get('education') else 'Relevant degree'}

# JOB DETAILS
Company: {company_name}
Required Skills: {', '.join(job_data.get('required_skills', [])[:5])}
Role Focus: {job_data.get('job_title', 'Position')}

# REQUIREMENTS
- Professional tone, not overly formal
- 3-4 paragraphs
- Highlight 2-3 relevant achievements
- Show enthusiasm for the role
- Include clear call to action
- Address to {hiring_manager}

# OUTPUT
Provide only the cover letter text, ready to use.
"""
        
        try:
            response = openai.chat.completions.create(
                model=self.model,
                messages=[
                    {
                        "role": "system",
                        "content": "You are an expert career coach specializing in compelling cover letters."
                    },
                    {
                        "role": "user",
                        "content": prompt
                    }
                ],
                temperature=0.8,
                max_tokens=600
            )
            
            return response.choices[0].message.content.strip()
        
        except Exception as e:
            print(f"Error generating cover letter: {e}")
            return f"""Dear {hiring_manager},

I am writing to express my strong interest in the {job_data.get('job_title', 'position')} at {company_name}. With my background in {', '.join(resume_data.get('skills', ['technology'])[:3])}, I am confident I can contribute meaningfully to your team.

Throughout my career, I have developed expertise in key areas that align with your requirements. My experience with {', '.join(job_data.get('required_skills', ['relevant technologies'])[:2])} has enabled me to deliver impactful results in previous roles.

I am particularly excited about {company_name}'s work and would welcome the opportunity to discuss how my skills and experience can benefit your team.

Thank you for considering my application. I look forward to speaking with you.

Sincerely,
[Your Name]"""


# Example usage
if __name__ == "__main__":
    # Initialize (requires OpenAI API key)
    generator = AIFeedbackGenerator(api_key="your-api-key-here")
    
    # Sample data
    resume_data = {
        'skills': ['python', 'tensorflow', 'aws'],
        'experience': [
            {'title': 'ML Engineer', 'company': 'Tech Co'}
        ],
        'education': [
            {'degree': 'MS Computer Science'}
        ]
    }
    
    job_data = {
        'required_skills': ['python', 'machine learning', 'aws', 'docker'],
        'preferred_skills': ['kubernetes'],
        'job_title': 'Senior ML Engineer'
    }
    
    score_details = {
        'overall_score': 72.5,
        'skill_match_score': 75.0,
        'missing_critical_skills': ['docker', 'kubernetes']
    }
    
    # Generate feedback
    feedback = generator.generate_comprehensive_feedback(
        resume_data,
        job_data,
        score_details
    )
    
    print("=== FEEDBACK REPORT ===")
    print(f"\nStrengths:")
    for s in feedback.strengths:
        print(f"  • {s}")
    
    print(f"\nWeaknesses:")
    for w in feedback.weaknesses:
        print(f"  • {w}")
    
    print(f"\nPriority Actions:")
    for a in feedback.priority_actions:
        print(f"  • {a}")
    
    # Rewrite a section
    original_bullet = "Worked on machine learning projects"
    improved = generator.rewrite_resume_section(
        original_bullet,
        "experience",
        job_data['required_skills']
    )
    
    print(f"\n=== REWRITE EXAMPLE ===")
    print(f"Original: {original_bullet}")
    print(f"Improved: {improved}")
    
    # ATS optimization
    ats_analysis = generator.optimize_for_ats(
        "Python developer with experience in machine learning",
        job_data['required_skills']
    )
    
    print(f"\n=== ATS ANALYSIS ===")
    print(f"ATS Score: {ats_analysis['ats_score']}/100")
    print(f"Missing Keywords: {', '.join(ats_analysis['missing_keywords'])}")